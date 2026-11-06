using System.Globalization;
using System.Text;
using LZ.ApiService.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Model.BaiWangModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using LZ.Core.Services;

namespace LZ.Finance.Service.Services;

/// <summary>沿用基础服务的阿里云 OCR 地址和认证方式，保留其 InvoiceModel 丢弃的验真要素。</summary>
public sealed class ReimbursementInvoiceRecognitionClient : IReimbursementInvoiceRecognitionClient, IScopedDependency
{
    private const string OcrUrl = "https://juminvoie.market.alicloudapi.com/ocr/invoice-v2";
    private readonly IAliyunOSSService _oss;
    private readonly IHttpService _http;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ISettingsService _settings;
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<ReimbursementInvoiceRecognitionClient> _logger;
    private readonly IHttpService _httpService;

    /// <summary>接入临时图片存储、HTTP 调用及既有阿里云和跨服务认证配置。</summary>
    /// <param name="oss">为 PDF 页面图片提供临时 OCR 地址，并在识别后删除图片。</param>
    /// <param name="http">通过服务发现解析发票服务地址。</param>
    /// <param name="httpClientFactory">创建保留传输错误的 HTTP 客户端。</param>
    /// <param name="settings">读取阿里云 AppCode、OSS 域名及内部服务令牌。</param>
    /// <param name="configuration">读取本地发票服务地址的可选覆盖配置。</param>
    /// <param name="environment">判定是否为无需服务发现的本地开发环境。</param>
    /// <param name="logger">记录不包含票面值的 OCR 字段诊断信息。</param>
    public ReimbursementInvoiceRecognitionClient(IAliyunOSSService oss, IHttpService http,
        IHttpClientFactory httpClientFactory,
        ISettingsService settings, IConfiguration configuration, IHostEnvironment environment,
        ILogger<ReimbursementInvoiceRecognitionClient> logger, IHttpService httpService)
    {
        _oss = oss;
        _http = http;
        _httpClientFactory = httpClientFactory;
        _settings = settings;
        _configuration = configuration;
        _environment = environment;
        _logger = logger;
        _httpService = httpService;
    }

    /// <summary>识别原始图片或 PDF 的每一页，提取百望云验真所需的字段。</summary>
    /// <param name="file">原始上传文件；PDF 由 PdfHelper.ConvertToImages 转为 PNG。</param>
    /// <param name="extension">小写文件扩展名，含前导点号。</param>
    /// <param name="fileUrl">原始文件的 OSS 地址；图片直接使用此地址。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>单张发票的识别结果；PDF 中识别出不同发票时要求分开上传。</returns>
    public async Task<ReimbursementRecognizedInvoice> RecognizeAsync(IFormFile file, string extension,
        string fileUrl, CancellationToken cancellationToken)
    {
        var settings = await _settings.GetMasterSettings();
        if (string.IsNullOrWhiteSpace(settings.AliAppCode) || string.IsNullOrWhiteSpace(settings.AliBucketDomain))
            throw new InvalidOperationException("发票识别服务未配置阿里云 AppCode 或 OSS 访问域名");
        if (extension != ".pdf")
            return await RecognizeImageAsync(fileUrl, settings.AliBucketDomain, settings.AliAppCode,
                cancellationToken);

        // 当前 OCR 接口只接受图片 URL。原 PDF 已保存；页面 PNG 只为识别提供临时地址。
        var images = PdfHelper.ConvertToImages(file);
        if (images.Count == 0) throw new InvalidOperationException("PDF 未包含可识别的发票页面");
        ReimbursementRecognizedInvoice? invoice = null;
        foreach (var image in images)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var objectKey = $"reimbursement/invoices/ocr/{Guid.NewGuid():N}.png";
            using (var stream = new MemoryStream(image))
            {
                var saved = await _oss.UploadOssFile(stream, objectKey);
                if (saved.Code != 0)
                    throw new InvalidOperationException("发票识别图片临时上传失败：" + saved.Message);
            }
            try
            {
                var current = await RecognizeImageAsync(objectKey, settings.AliBucketDomain,
                    settings.AliAppCode, cancellationToken);
                if (invoice is not null && invoice != current)
                    throw new InvalidOperationException("PDF 包含多张不同发票，请逐张上传");
                invoice = current;
            }
            finally
            {
                try
                {
                    if (!await _oss.Delete(objectKey))
                        _logger.LogWarning("发票识别临时图片清理失败：{ObjectKey}", objectKey);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "发票识别临时图片清理失败：{ObjectKey}", objectKey);
                }
            }
        }
        return invoice ?? throw new InvalidOperationException("发票识别失败");
    }

    /// <summary>按图片 URL 调用现有阿里云 OCR 接口并解析验真要素。</summary>
    /// <param name="imageUrl">原图片地址或 PDF 页面图片的临时 OSS 对象键。</param>
    /// <param name="bucketDomain">OSS 公开访问域名。</param>
    /// <param name="appCode">阿里云市场 OCR 接口的授权码。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>单张图片识别出的发票信息。</returns>
    private async Task<ReimbursementRecognizedInvoice> RecognizeImageAsync(string imageUrl,
        string bucketDomain, string appCode, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var absoluteUrl = Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri) &&
                          uri.Scheme is "http" or "https"
            ? imageUrl : bucketDomain.TrimEnd('/') + "/" + imageUrl.TrimStart('/');
        using var content = new FormUrlEncodedContent(new Dictionary<string, string> { ["url"] = absoluteUrl });
        var raw = await PostAsync(OcrUrl, content, "APPCODE " + appCode, "发票识别", cancellationToken);
        JObject body;
        try { body = JObject.Parse(raw); }
        catch (JsonReaderException ex) { throw new InvalidOperationException("发票识别接口返回的数据不是有效 JSON", ex); }
        JObject? data;
        try
        {
            data = body["data"] is JObject obj ? obj :
                body["data"]?.Type == JTokenType.String ? JObject.Parse(body["data"]!.ToString()) : null;
        }
        catch (JsonReaderException ex)
        {
            throw new InvalidOperationException("发票识别接口返回的票面数据不是有效 JSON", ex);
        }
        if (Value(body, "code") != "200" || data is null)
        {
            var code = Value(body, "code");
            var message = Value(body, "msg", "message");
            throw new InvalidOperationException("发票识别接口未返回有效票面"
                + (string.IsNullOrWhiteSpace(code) ? "" : "，错误码：" + code)
                + (string.IsNullOrWhiteSpace(message) ? "" : "，原因：" + message));
        }
        var number = Value(data, "invoiceNum", "invoiceNumber", "invoiceNo");
        var taxNo = Value(data, "buyerTaxNo", "buyerTaxNumber", "buyerTaxId", "buyerTaxCode",
            "purchaserTaxNumber", "purchaserTaxNo", "purchaserTaxId", "purchaserTaxCode",
            "purchaserTaxpayerIdentificationNumber", "PurchaserRegisterNum", "purchaser_register_num");
        var dateText = Value(data, "invoiceDate", "billingDate", "issueDate", "date");
        var amountText = Value(data, "amountInFiguers", "amountInFigures", "totalAmount",
            "invoiceTotalPriceTax");
        // 分开校验，避免前面的短路条件使调试器将未执行 TryParse 的 amount 显示为 0。
        if (string.IsNullOrWhiteSpace(number) || number.Length > 20)
            throw RecognitionFailure("发票号码缺失或超过 20 位", data);
        if (string.IsNullOrWhiteSpace(taxNo) || taxNo.Length > 20)
            throw RecognitionFailure("购方税号缺失或超过 20 位", data);
        if (!TryDate(dateText, out var billingDate))
            throw RecognitionFailure("开票日期缺失或格式无法解析", data);
        var normalizedAmount = amountText.Replace("¥", "").Replace("￥", "").Replace(",", "");
        if (!decimal.TryParse(normalizedAmount, NumberStyles.Number,
                CultureInfo.InvariantCulture, out var amount) ||
            amount <= 0 || decimal.Round(amount, 2) != amount)
            throw RecognitionFailure("价税合计金额缺失或格式无效", data);
        return new ReimbursementRecognizedInvoice(number, taxNo,
            billingDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), amount);
    }

    /// <summary>记录 OCR 缺失字段的诊断信息，不记录发票号码、税号等票面值。</summary>
    /// <param name="reason">无法继续验真的原因。</param>
    /// <param name="data">OCR 返回的数据对象，仅记录字段名称。</param>
    /// <returns>供上传流程标记识别失败的异常。</returns>
    private InvalidOperationException RecognitionFailure(string reason, JObject data)
    {
        _logger.LogWarning("发票 OCR 字段校验失败：{Reason}；返回字段：{Fields}", reason,
            string.Join(",", data.Properties().Select(property => property.Name)));
        return new InvalidOperationException("发票识别失败：" + reason);
    }

    /// <summary>跨服务调用百望云验真接口，仅接受状态正常且号码、金额与 OCR 一致的票面。</summary>
    /// <param name="invoice">OCR 识别的验真要素；购方税号保留用于核对百望云返回的真实票面。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>通过验真的发票号码和价税合计金额。</returns>
    public async Task<ReimbursementVerifiedInvoice> VerifyAsync(ReimbursementRecognizedInvoice invoice,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var request = new BaiWangInvoiceValidationRequest
        {
            taxNo = invoice.BuyerTaxNo,
            invoiceNumber = invoice.InvoiceNumber,
            billingDate = invoice.BillingDate,
            invoiceType = "32",
            totalAmount = invoice.TotalAmount.ToString("0.00", CultureInfo.InvariantCulture)
        };
        // 联调环境使用百望云测试税号；若测试部署也标为 Production，可显式开启测试开关。
        //var useTestBuyerTaxNo = bool.TryParse(
        //    _configuration["Reimbursement:InvoiceVerification:UseTestBuyerTaxNo"], out var enabled) && enabled;
        //if (!_environment.IsProduction() || useTestBuyerTaxNo)
        //TODO 测试环境想用指定的购方房票税号，生产环境到时候会注释到用发票实际识别到的 
        request.taxNo = "9122222222222B0647";
        var settings = await _settings.GetMasterSettings();
        if (string.IsNullOrWhiteSpace(settings.ApiToken))
            throw new InvalidOperationException("发票验真服务未配置内部接口令牌");
        // 开发环境直连本地发票 API；测试和生产环境经 Consul 发现 InvoiceService。
        // BaiWangController 返回 ContentResult，故读取原始 JSON，不能用会期待 RequestResultModel 的 PostAsync。
        using var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        _logger.LogError($"[测试排查] invoice base url is：{GetInvoiceServiceBaseUrl()}");
        Common.StaticLog.Debug($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] invoice base url is：{GetInvoiceServiceBaseUrl()}");

        //var verifyInvoicePath = _environment.IsDevelopment() ? "/api/BaiWang/VerifyInvoiceAuthenticity" : "/api/invoice/BaiWang/VerifyInvoiceAuthenticity";
        //_httpService.PostAsync(ServiceNames.InvoiceService, "api/BaiWang/VerifyInvoiceAuthenticity", JsonConvert.SerializeObject(request));
        var verifyInvoiceUrl = GetInvoiceServiceBaseUrl() + "/api/BaiWang/VerifyInvoiceAuthenticity";
        _logger.LogError($"[测试排查] verifyInvoiceUrl is：{verifyInvoiceUrl}");
        var raw = await PostAsync(GetInvoiceServiceBaseUrl() + "/api/BaiWang/VerifyInvoiceAuthenticity",
            content, "Basic " + settings.ApiToken, "发票验真", cancellationToken);
        BaiWangResponse<BaiWangInvoiceValidationResponse>? result;
        try { result = JsonConvert.DeserializeObject<BaiWangResponse<BaiWangInvoiceValidationResponse>>(raw); }
        catch (JsonException ex) { throw new InvalidOperationException("发票验真接口返回的数据不是有效 JSON", ex); }
        // 例如 success=false、response=null、errorResponse.code=100006，应在这里处理。
        if (result?.success == false)
        {
            _logger.LogWarning("百望云发票查验接口返回错误：Code={Code}，SubCode={SubCode}，RequestId={RequestId}",
                result.errorResponse?.code, result.errorResponse?.subCode, result.requestId);
            if (result.errorResponse?.code == "100006" &&
                result.errorResponse.subMessage?.Contains("无权操作此税号信息", StringComparison.Ordinal) == true)
                throw new ReimbursementInvoiceAuthorizationException();
            var error = result.errorResponse;
            var reason = !string.IsNullOrWhiteSpace(error?.subMessage) ? error.subMessage : error?.message;
            throw new InvalidOperationException("百望云发票查验失败"
                + (string.IsNullOrWhiteSpace(error?.code) ? "" : "，错误码：" + error.code)
                + (string.IsNullOrWhiteSpace(reason) ? "" : "，原因：" + reason));
        }
        var data = result?.response?.data;
        if (result?.success != true || data is null)
            throw new InvalidOperationException("发票验真接口未返回票面数据");
        var verifiedNumber = Value(data, "invoiceNumber", "invoiceNo", "digitInvoiceNo");
        var status = Value(data, "state", "invoiceStatus", "invoiceBaseStatus", "status");
        if (string.IsNullOrWhiteSpace(verifiedNumber))
            throw new InvalidOperationException("发票验真结果缺少发票号码");
        if (!string.Equals(verifiedNumber, invoice.InvoiceNumber, StringComparison.Ordinal))
            throw new InvalidOperationException("发票验真结果的发票号码与识别结果不一致");
        if (string.IsNullOrWhiteSpace(status))
            throw new InvalidOperationException("发票验真结果缺少发票状态");
        if (status is not ("0" or "1" or "01" or "正常" or "normal" or "NORMAL"))
            throw new InvalidOperationException("发票验真结果的发票状态异常：" + status);
        var buyerTaxNo = Value(data, "buyerTaxNo", "purchaserTaxNo", "purchaserTaxNumber");
        var billingDate = Value(data, "billingDate", "invoiceDate", "issueDate");
        //TODO 现在获取到的是测试环境的购方税号，后续生产环境需要注释掉
        buyerTaxNo = request.taxNo;
        if (!string.IsNullOrEmpty(buyerTaxNo) &&
            !string.Equals(buyerTaxNo, request.taxNo, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("发票验真结果的购方税号与查验请求不一致");
        if (!string.IsNullOrEmpty(billingDate) &&
            (!TryDate(billingDate, out var verifiedDate) ||
             verifiedDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) != invoice.BillingDate))
            throw new InvalidOperationException("发票验真结果的开票日期与识别结果不一致");
        // 百望云此票面的 amountTax 为价税合计，totalAmount 为不含税金额。
        var amountText = Value(data, "amountTax", "invoiceTotalPriceTax", "totalPriceTax", "amountInFiguers",
            "totalAmount");
        if (!decimal.TryParse(amountText.Replace("¥", "").Replace("￥", "").Replace(",", ""),
                NumberStyles.Number, CultureInfo.InvariantCulture, out var verifiedAmount) ||
            verifiedAmount <= 0 || decimal.Round(verifiedAmount, 2) != verifiedAmount)
            throw new InvalidOperationException("发票验真结果缺少有效的价税合计金额");
        if (verifiedAmount != invoice.TotalAmount)
            throw new InvalidOperationException("发票验真结果的价税合计金额与识别结果不一致");
        return new ReimbursementVerifiedInvoice(verifiedNumber, verifiedAmount);
    }

    private async Task<string> PostAsync(string url, HttpContent content, string authorization,
        string serviceName, CancellationToken cancellationToken)
    {
        using var client = _httpClientFactory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
        request.Headers.TryAddWithoutValidation("Authorization", authorization);
        try
        {
            using var response = await client.SendAsync(request, cancellationToken);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                string reason;
                try { reason = Value(JObject.Parse(body), "message", "msg", "error_description"); }
                catch (JsonReaderException) { reason = string.Empty; }
                if (reason.Length > 120) reason = reason[..120];
                throw new InvalidOperationException($"{serviceName}接口返回 HTTP {(int)response.StatusCode} {response.ReasonPhrase}"
                    + (string.IsNullOrWhiteSpace(reason) ? "" : "：" + reason));
            }
            return body;
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"{serviceName}请求失败：{ex.Message}", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new InvalidOperationException($"{serviceName}请求超时：{ex.Message}", ex);
        }
    }

    /// <summary>解析发票服务地址；本地开发直连，其他环境使用服务发现。</summary>
    /// <returns>不带末尾斜杠的发票服务根地址。</returns>
    private string GetInvoiceServiceBaseUrl()
    {
        _logger.LogError($"[测试排查] current environment: {_environment.EnvironmentName}");
        Common.StaticLog.Debug($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] current environment: {_environment.EnvironmentName}");
        if (!_environment.IsDevelopment())
            return _http.GetServicePath("InvoiceService").TrimEnd('/');

        // 与 LZ.Invoice.Api 的本地 http 启动配置一致；可通过环境变量
        // Reimbursement__InvoiceServiceUrl 覆盖，无需修改任何项目配置文件。
        var localUrl = _configuration["Reimbursement:InvoiceServiceUrl"];
        return (string.IsNullOrWhiteSpace(localUrl) ? "http://localhost:8873" : localUrl).TrimEnd('/');
    }

    /// <summary>按候选字段名读取供应商 JSON 中首个非空值，忽略字段名大小写。</summary>
    /// <param name="data">供应商返回的 JSON 对象。</param>
    /// <param name="names">不同票种或供应商版本可能使用的字段名。</param>
    /// <returns>首个非空字段值；均不存在时返回空字符串。</returns>
    private static string Value(JObject data, params string[] names)
    {
        foreach (var name in names)
        {
            var value = data.GetValue(name, StringComparison.OrdinalIgnoreCase);
            if (value is not null && value.Type != JTokenType.Null && !string.IsNullOrWhiteSpace(value.ToString()))
                return value.ToString().Trim();
        }
        return string.Empty;
    }

    /// <summary>将 OCR 或验真票面的日期解析为日期值。</summary>
    /// <param name="value">票面日期文本。</param>
    /// <param name="date">解析成功后的日期；失败时为默认值。</param>
    /// <returns>日期文本是否符合支持的格式。</returns>
    private static bool TryDate(string value, out DateTime date) =>
        DateTime.TryParseExact(value, new[] { "yyyy-MM-dd", "yyyy/MM/dd", "yyyyMMdd", "yyyy年M月d日" },
            CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
}

/// <summary>百望云明确拒绝当前 appKey 查验购方税号；不能据此认定发票真伪。</summary>
internal sealed class ReimbursementInvoiceAuthorizationException : InvalidOperationException
{
    /// <summary>创建不包含购方税号的授权错误，供上传流程写入可操作的失败提示。</summary>
    public ReimbursementInvoiceAuthorizationException()
        : base("百望云未授权查验该购方税号")
    {
    }
}
