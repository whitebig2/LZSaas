using LZ.Core.Attributes;
using LZ.Finance.Service.Interface;
using LZ.Model.Dtos.Reimbursement;
using LZ.Model.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LZ.Reimbursement.API.Controllers;

[PlatformLimit(PlatformType.XFZMini)]
[PlatformLimit(PlatformType.LZMini)]
[PlatformLimit(PlatformType.MiniAdmin)]
public sealed class MiniReimbursementReadController : ReimbursementControllerBase
{
    private readonly IReimbursementReadService _read;
    private readonly IReimbursementWriteService _write;
    private readonly IReimbursementInvoiceService _invoices;

    /// <summary>接入本人申请查询、申请写入和发票处理服务。</summary>
    /// <param name="read">本人申请及发票查询服务。</param>
    /// <param name="write">申请提交、重提、补票服务。</param>
    /// <param name="invoices">发票上传和字典服务。</param>
    public MiniReimbursementReadController(IReimbursementReadService read,
        IReimbursementWriteService write, IReimbursementInvoiceService invoices)
    {
        _read = read;
        _write = write;
        _invoices = invoices;
    }

    [HttpGet]
    public async Task<IActionResult> GetDepartmentInfo()
    {
        var currentUser = CurrentUser;
        var dept = await _read.GetCurrentPartnerAsync(currentUser.CompanyGID.Value, currentUser.UserID);
        return Ok(new { dept?.PartnerGID, dept.IsBudget });

    }

    /// <summary>查询法人主体选项。</summary>
    /// <param name="keyword">可选的名称搜索词。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>法人主体字典项。</returns>
    [HttpGet]
    public Task<IActionResult> LegalEntities([FromQuery] string? keyword, CancellationToken cancellationToken) =>
        DictionaryAsync("legal-entities", keyword, null, cancellationToken);

    /// <summary>查询成本类型选项。</summary>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>成本类型字典项。</returns>
    [HttpGet]
    public Task<IActionResult> CostTypes(CancellationToken cancellationToken) =>
        DictionaryAsync("cost-types", null, null, cancellationToken);

    /// <summary>按成本类型查询费用名称选项。</summary>
    /// <param name="costTypeId">可选的成本类型 ID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>费用名称字典项。</returns>
    [HttpGet]
    public Task<IActionResult> ExpenseNames([FromQuery] int? costTypeId, CancellationToken cancellationToken) =>
        DictionaryAsync("expense-names", null, costTypeId, cancellationToken);

    /// <summary>查询收款单位开户行选项。</summary>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>银行字典项。</returns>
    [HttpGet]
    public Task<IActionResult> PayeeBanks(CancellationToken cancellationToken) =>
        DictionaryAsync("payee-banks", null, null, cancellationToken);

    /// <summary>上传首次申请或补传发票，并发起异步识别。</summary>
    /// <param name="request">文件、上传用途；补传时须包含申请 GUID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>上传 GUID 和当前识别状态。</returns>
    [HttpPost]
    [Consumes("multipart/form-data")]
    public Task<IActionResult> UploadInvoice([FromForm] InvoiceUploadRequest request,
        CancellationToken cancellationToken) =>
        ExecuteAsync(async () => ToCamelCase(Ok(await _invoices.UploadAsync(request, cancellationToken))));

    /// <summary>查询当前申请人所属部门使用审核版还是预算版流程。</summary>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>流程版本及其判定来源。</returns>
    [HttpGet]
    public Task<IActionResult> GetFlowVersion(CancellationToken cancellationToken) =>
        ExecuteAsync(async () => ToCamelCase(Ok(await _write.GetFlowVersionAsync(cancellationToken))));

    /// <summary>提交首次报销或付款申请；申请类型由请求体的 ApplyType 决定。</summary>
    /// <param name="request">JSON 请求体。包含法人主体、成本类型、费用及收款信息、申请金额、申请类型、发票上传 GUID 和幂等键；各字段用途见 ApplicationCreateRequest 及其基类 ApplicationFieldsRequest。</param>
    /// <param name="cancellationToken">HTTP 请求取消令牌，客户端中断请求时用于终止后续异步处理；无需在 JSON 中传入。</param>
    /// <returns>提交结果，包含申请 GUID、单号、申请状态、流程版本以及是否生成待补票异常。</returns>
    [HttpPost]
    public Task<IActionResult> Submit([FromBody] ApplicationCreateRequest request, CancellationToken cancellationToken) =>
        ExecuteAsync(async () => ToCamelCase(Ok(await _write.SubmitAsync(request, cancellationToken))));

    /// <summary>再次申请：审核退回时重提原单，已打款时复用原票创建新单。</summary>
    /// <param name="request">源申请 GUID、修改后的申请字段及幂等键。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>重提或新建申请的标识和状态。</returns>
    [HttpPost]
    public Task<IActionResult> Copy([FromBody] ApplicationCopyRequest request, CancellationToken cancellationToken) =>
        ExecuteAsync(async () => ToCamelCase(Ok(await _write.CopyAsync(request, cancellationToken))));

    /// <summary>将审核退回的原申请修改后重新提交审批。</summary>
    /// <param name="applyGuid">原申请 GUID。</param>
    /// <param name="request">新表单字段、可追加的发票上传 GUID 和幂等键。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>原申请的最新状态。</returns>
    [HttpPut]
    public Task<IActionResult> Resubmit(Guid applyGuid, [FromBody] ApplicationResubmitRequest request,
        CancellationToken cancellationToken) =>
        ExecuteAsync(async () => ToCamelCase(Ok(await _write.ResubmitAsync(applyGuid, request, cancellationToken))));

    /// <summary>为待补票的付款申请关联新发票并重算缺口。</summary>
    /// <param name="applyGuid">付款申请 GUID。</param>
    /// <param name="request">已完成识别的补传上传 GUID 和幂等键。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>累计有效票额和是否补齐。</returns>
    [HttpPost]
    public Task<IActionResult> Supplement(Guid applyGuid, [FromBody] SupplementInvoiceRequest request,
        CancellationToken cancellationToken) =>
        ExecuteAsync(async () => ToCamelCase(Ok(await _write.SupplementAsync(applyGuid, request, cancellationToken))));

    /// <summary>分页查询当前用户的报销及付款申请列表。</summary>
    /// <param name="query">状态、费用名称、日期、关键字和分页条件。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>本人申请分页及可用操作。</returns>
    [HttpGet]
    public Task<IActionResult> GetApplications([FromQuery] ApplicationQuery query, CancellationToken cancellationToken) =>
        ExecuteAsync(async () => ToCamelCase(Ok(await _read.GetApplicationsAsync(query, cancellationToken))));

    /// <summary>查询本人申请详情、发票、审批轨迹及打款信息。</summary>
    /// <param name="applyGuid">申请 GUID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>申请详情；不存在时返回 404。</returns>
    [HttpGet]
    public Task<IActionResult> GetApplication(Guid applyGuid, CancellationToken cancellationToken) =>
        ExecuteAsync(async () =>
        {
            var detail = await _read.GetApplicationAsync(applyGuid, cancellationToken);
            return detail is null ? Error(404, "申请不存在") : ToCamelCase(Ok(detail));
        });

    /// <summary>读取再次申请所需的原表单和只读发票信息。</summary>
    /// <param name="applyGuid">审核退回或已打款的源申请 GUID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>可修改的原申请数据及发票快照。</returns>
    [HttpGet]
    public Task<IActionResult> GetResubmitPreload(Guid applyGuid, CancellationToken cancellationToken) =>
        ExecuteAsync(async () =>
        {
            var preload = await _read.GetResubmitPreloadAsync(applyGuid, cancellationToken);
            return preload is null ? Error(404, "申请不存在") : ToCamelCase(Ok(preload));
        });

    /// <summary>查询本人申请对应的打款确认记录。</summary>
    /// <param name="applyGuid">申请 GUID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>打款单；不存在时返回 404。</returns>
    [HttpGet]
    public Task<IActionResult> GetPaymentOrder(Guid applyGuid, CancellationToken cancellationToken) =>
        ExecuteAsync(async () =>
        {
            var order = await _read.GetPaymentOrderAsync(applyGuid, cancellationToken);
            return order is null ? Error(404, "打款单不存在") : ToCamelCase(Ok(order));
        });

    /// <summary>轮询本人发票上传记录的识别与验真结果。</summary>
    /// <param name="uploadGuid">上传记录 GUID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>识别、验真、查重状态及有效金额。</returns>
    [HttpGet]
    public Task<IActionResult> GetInvoiceUpload(Guid uploadGuid, CancellationToken cancellationToken) =>
        ExecuteAsync(async () =>
        {
            var upload = await _read.GetInvoiceUploadAsync(uploadGuid, cancellationToken);
            return upload is null ? Error(404, "发票上传记录不存在") : ToCamelCase(Ok(upload));
        });

    /// <summary>提交申请前预校验所选发票的有效金额；此操作不绑定发票。</summary>
    /// <param name="request">上传 GUID、申请金额、申请类型及可选流程版本。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>有效票额、有效/无效上传 GUID 和提示。</returns>
    [HttpPost]
    public Task<IActionResult> ValidateInvoices([FromBody] InvoiceValidationRequest request,
        CancellationToken cancellationToken) =>
        ExecuteAsync(async () => ToCamelCase(Ok(await _read.ValidateInvoicesAsync(request, cancellationToken))));

    /// <summary>调用字典扩展服务，并按项目约定返回小驼峰查询结果。</summary>
    /// <param name="kind">字典种类。</param>
    /// <param name="keyword">可选搜索词。</param>
    /// <param name="costTypeId">可选成本类型 ID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>字典项响应。</returns>
    private Task<IActionResult> DictionaryAsync(string kind, string? keyword, int? costTypeId,
        CancellationToken cancellationToken) =>
        ExecuteAsync(async () => ToCamelCase(Ok(await _invoices.GetDictionaryAsync(kind, keyword, costTypeId,
            cancellationToken))));

    /// <summary>执行接口逻辑并把业务异常映射为 HTTP 错误响应。</summary>
    /// <param name="action">待执行的异步接口逻辑。</param>
    /// <returns>接口响应。</returns>
    private async Task<IActionResult> ExecuteAsync(Func<Task<IActionResult>> action)
    {
        try { return await action(); }
        catch (ArgumentException ex) { return Error(400, ex.Message); }
        catch (UnauthorizedAccessException ex) { return Error(403, ex.Message); }
        catch (InvalidOperationException ex) { return Error(409, ex.Message); }
    }

    /// <summary>保留原有业务错误的 HTTP 状态码和响应内容。</summary>
    /// <param name="status">HTTP 状态码。</param>
    /// <param name="message">错误说明。</param>
    /// <returns>业务错误响应。</returns>
    private IActionResult Error(int status, string message) => new JsonResult(new ApiResponse<object>
    {
        Code = status,
        Message = message,
        TraceId = HttpContext.TraceIdentifier
    })
    { StatusCode = status };
}
