using LZ.ApiService.Interface;
using LZ.Invoice.Service.Interface;
using LZ.Model;
using LZ.Model.BaiWangModel;
using LZ.Model.Enums.Invoice;
using LZ.Model.Exceptions;
using LZ.Model.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Globalization;
using System.Text.Json;

namespace LZ.Invoice.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BaiWangController : InvoiceControllerBase
    {
        private readonly IBaiWangService _baiWangService;
        private readonly IInvoiceInfoService _invoiceInfoService;
        private readonly IPartnerAuthCodeService _partnerAuthCodeService;
        private readonly BaiWangOption _option;

        public BaiWangController(IBaiWangService baiWangService, IInvoiceInfoService invoiceInfoService, IPartnerAuthCodeService partnerAuthCodeService, IOptions<BaiWangOption> option)
        {
            _baiWangService = baiWangService;
            _invoiceInfoService = invoiceInfoService;
            _partnerAuthCodeService = partnerAuthCodeService;
            _option = option.Value;
        }

        /// <summary>
        /// 百望发票用户授权获取 Token
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetToken(bool cacheToken = true)
        {
            var token = await _baiWangService.GetTokenAsync(cacheToken);
            return Ok(token);
        }

        /// <summary>
        /// 通过百望云查验发票真伪，并返回查验结果及完整票面信息。
        /// </summary>
        /// <param name="request">发票查验要素；普通票提供校验码，其他票种按文档提供对应金额。</param>
        /// <returns>百望云原始业务响应；success 表示接口调用成功，票面状态以 response.data 为准。</returns>
        [HttpPost]
        public async Task<IActionResult> VerifyInvoiceAuthenticity([FromBody] BaiWangInvoiceValidationRequest request)
        {
            // Service 通过现有 OAuth 流程获取 token，并按开放平台要求签名和提交请求。
            var result = await _baiWangService.VerifyInvoiceAuthenticityAsync(request);

            // data 包含大量随票种变化的字段，直接输出原始 JSON，避免丢失全票面信息。
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }

        /// <summary>
        /// 创建开票申请
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddPreInvoice([FromBody] BaiWangAddPreInvoiceRequest request)
        {
            var result = await _baiWangService.AddPreInvoiceAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// 查询开票申请
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> QueryPreInvoice([FromBody] BaiWangQueryPreInvoiceRequest request)
        {
            var result = await _baiWangService.QueryPreInvoiceAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// 查询发票
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> QueryInvoice([FromBody] BaiWangQueryInvoiceRequest request)
        {
            var result = await _baiWangService.QueryInvoiceAsync(request,"");
            return Ok(result);
        }

        /// <summary>
        /// 开具发票
        /// </summary>
        //[HttpPost]
        //public async Task<IActionResult> IssueInvoice([FromBody] BaiWangIssueInvoiceRequest request)
        //{

        //    var result = await _baiWangService.IssueInvoiceAsync(request);
        //    return Ok(result);
        //}
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> IssueInvoice([FromForm]Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ErrorCodeException(-1, "参数错误");
            }
            var info = await _invoiceInfoService.Get(id.Value);
            if (!info.GoodsTotalPriceTax.HasValue)
            {
                throw new ErrorCodeException(-1, "金额错误");
            }
            info.OrderNo = "LB" + DateTime.Now.ToString("yyyyMMddHHmmss");
            info.OrderDateTime = DateTime.Now;
            decimal rate = info.GoodsTaxRate.HasValue ? info.GoodsTaxRate.Value : 0M;
            decimal goodsTaxRate = Math.Round((rate / 100), 2); //税率
            decimal goodsTotalPriceTax = info.GoodsTotalPriceTax.Value;//含税金额
            decimal deductAmount = info.DeductibleAmount.HasValue ? info.DeductibleAmount.Value : 0M;
            decimal goodsTotalPrice = 0M;
            decimal goodsTotalTax = 0M;
            if (info.TaxationLabel == "01")
            {
                goodsTotalPrice = Math.Round(goodsTotalPriceTax / (1M + goodsTaxRate), 2);//不含税金额
                goodsTotalTax = Math.Round(goodsTotalPrice * goodsTaxRate, 2);
            }
            else
            {
                
                decimal heji = goodsTotalPriceTax - deductAmount;
                goodsTotalPrice = Math.Round(heji / (1M + goodsTaxRate), 2);//不含税金额
                goodsTotalPrice = deductAmount + goodsTotalPrice;
                goodsTotalTax = goodsTotalPriceTax - goodsTotalPrice;
            }
            
            info.GoodsTotalPriceTax = goodsTotalPriceTax;
            info.GoodsTotalPrice = goodsTotalPrice;
            info.GoodsTotalTax = goodsTotalTax;
            if (rate > 100)
            {
                rate = 100;
            }
            
            List <BaiWangInvoiceDetail> details = new List<BaiWangInvoiceDetail>();
            BaiWangInvoiceDetail detail = new BaiWangInvoiceDetail()
            {
                goodsName = info.GoodsName,
                goodsCode = info.GoodsCode,
                goodsTaxRate = goodsTaxRate.ToString(),
                goodsTotalPriceTax = goodsTotalPriceTax.ToString(),
                goodsTotalPrice = goodsTotalPrice.ToString(),
                goodsTotalTax = goodsTotalTax.ToString()

            };
            details.Add(detail);
            BaiWangIssueInvoiceRequest request = new BaiWangIssueInvoiceRequest()
            {
                taxNo = info.TaxNo,
                orderNo = info.OrderNo,
                orderDateTime = info.OrderDateTime.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                invoiceTypeCode = info.InvoiceTypeCode,
                invoiceType = "1",//1-蓝票,2-红票
                priceTaxMark = "1",//单价含税标志:0-不含税,1 - 含税
                buyerName = info.BuyerName,
                buyerTaxNo = info.BuyerTaxNo,
                pushEmail = info.PushEmail,
                pushPhone = info.PushPhone,
                invoiceDetailList = details
            };

            //callBackUrl = _option.HostUrl + "/api/invoice/BaiWang/InvoiceCallback",
            if (info.TaxationLabel == "02")
            {
                request.taxationLabel = info.TaxationLabel;
            }
            if (!string.IsNullOrEmpty(info.SellerBankName))
            {
                request.sellerBankName = info.SellerBankName;
            }
            if (!string.IsNullOrEmpty(info.SellerBankNumber))
            {
                request.sellerBankNumber = info.SellerBankNumber;
            }
            if (!string.IsNullOrEmpty(info.BuyerBankName))
            {
                request.buyerBankName = info.BuyerBankName;
            }
            if (!string.IsNullOrEmpty(info.BuyerBankNumber))
            {
                request.buyerBankNumber = info.BuyerBankNumber;
            }
            if (request.taxationLabel == "02")
            {
                List<BaiWangBalanceInfoList> balanceInfoList = new List<BaiWangBalanceInfoList>();
                BaiWangBalanceInfoList balance = new BaiWangBalanceInfoList();
                //if(request.invoiceTypeCode == "01")
                //{
                //    balance.balanceType = "10";
                //}
                //else if (request.invoiceTypeCode == "02")
                //{
                //    balance.balanceType = "11";
                //}
                balance.balanceType = "09";
                balance.balanceTotalAmount = detail.goodsTotalPriceTax;
                //balance.balanceTotalAmount = totalTax.ToString();
                balance.balanceDeductAmount = deductAmount.ToString();
                balanceInfoList.Add(balance);
                request.balanceInfoList = balanceInfoList;
            }

            var partnerAuthCode = await _partnerAuthCodeService.Get(t => t.TaxNo == info.TaxNo);
            if (partnerAuthCode == null)
            {
                throw new ErrorCodeException(-1, "开票方未收到邀请，请联系管理员");
            }




            var result = await _baiWangService.IssueInvoiceAsync(request, partnerAuthCode.OrgAuthCode);
            if (result.success)
            {
                info.Status = (int)InvoiceStatusEnum.开具中;
                info.SerialNo = result.response.serialNo;
                info.Msg = "开具申请成功";
            }
            else
            {
                info.Status = (int)InvoiceStatusEnum.开具失败;
                info.Msg = result.errorResponse.subMessage;
            }

                await _invoiceInfoService.Update(info);
            return Ok(new { request,result,info });
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> InvoiceQuery([FromForm] Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ErrorCodeException(-1, "参数错误");
            }
            var info = await _invoiceInfoService.Get(id.Value);
            BaiWangQueryInvoiceRequest request = new BaiWangQueryInvoiceRequest();
            request.taxNo = info.TaxNo ;
            request.orderNo = info.OrderNo;
            var partnerAuthCode = await _partnerAuthCodeService.Get(t => t.TaxNo == info.TaxNo);
            if (partnerAuthCode == null)
            {
                throw new ErrorCodeException(-1, "开票方未收到邀请，请联系管理员");
            }
            var result = await _baiWangService.QueryInvoiceAsync(request, partnerAuthCode.OrgAuthCode);
            if(result.success)
            {
                if(result.response.list.Count > 0)
                {
                    var data = result.response.list[0];
                    //开票状态:00-开票中,01 - 开票完成,02 - 开票失败,03 - 发票已作废,04 - 发票作废中
                    if (data.status == "00")
                    {
                        info.Status = (int)InvoiceStatusEnum.开具中;
                    }
                    else if (data.status == "01")
                    {
                        info.Status = (int)InvoiceStatusEnum.已开具;
                    }
                    else if (data.status == "02")
                    {
                        info.Status = (int)InvoiceStatusEnum.开具失败;
                    }
                    info.InvoiceBaseStatus = data.invoiceBaseStatus;
                    info.DigitInvoiceNo = data.digitInvoiceNo;
                    info.Msg = "开票结果已出";
                    await _invoiceInfoService.Update(info);
                }
                

            }
            return Ok(new { request, result,info });
        }




        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> InvoiceFastRed([FromForm] Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ErrorCodeException(-1, "参数错误");
            }
            var info = await _invoiceInfoService.Get(id.Value);
            if (info.Status!=(int)InvoiceStatusEnum.已开具)
            {
                throw new ErrorCodeException(-1, "发票未开具，不能冲红");
            }
            info.RedOrderNo = "LBRed" + DateTime.Now.ToString("yyyyMMddHHmmss");
            BaiWangFastRedRequest request = new BaiWangFastRedRequest()
            {
                taxNo = info.TaxNo,
                orderNo = info.RedOrderNo,
                invoiceTerminalCode = info.InvoiceTerminalCode,
                originalSerialNo = info.SerialNo,
                originalDigitInvoiceNo = info.DigitInvoiceNo,
                pushEmail = info.PushEmail,
                pushPhone = info.PushPhone
            };
            var partnerAuthCode = await _partnerAuthCodeService.Get(t => t.TaxNo == info.TaxNo);
            if (partnerAuthCode == null)
            {
                throw new ErrorCodeException(-1, "开票方未收到邀请，请联系管理员");
            }
            var result = await _baiWangService.IssueFastRedAsync(request,partnerAuthCode.OrgAuthCode);
            if (result.success)
            {
                info.RedConfirmSerialNo = result.response.redConfirmSerialNo;
                info.Status = (int)InvoiceStatusEnum.冲红中;
                info.Msg = "冲红申请成功";
            }
            else
            {
                info.Status = (int)InvoiceStatusEnum.冲红失败;
                info.Msg = result.errorResponse.subMessage;
            }

            await _invoiceInfoService.Update(info);


            return Ok(new { request, result });

        }

        /// <summary>
        /// 红票确认单
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> InvoiceRedConfirm([FromForm] Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ErrorCodeException(-1, "参数错误");
            }
            var info = await _invoiceInfoService.Get(id.Value);
            BaiWangRedConfirmRequest request = new BaiWangRedConfirmRequest()
            {
                taxNo = info.TaxNo,
                operatorType = "2",
                buySelSelector = "0",
                redConfirmSerialNo = info.RedConfirmSerialNo,
                originalDigitInvoiceNo = info.DigitInvoiceNo
            };
            var partnerAuthCode = await _partnerAuthCodeService.Get(t => t.TaxNo == info.TaxNo);
            if (partnerAuthCode == null)
            {
                throw new ErrorCodeException(-1, "开票方未收到邀请，请联系管理员");
            }
            var result = await _baiWangService.IssueRedConfirmAsync(request,partnerAuthCode.OrgAuthCode);
            //return Ok(new { request, result });
            if (result.success)
            {
                if (result.response.list.Count > 0)
                {
                    var data = result.response.list[0];
                    if(data.redConfirmStatus == "01" || data.redConfirmStatus == "04")
                    {
                        BaiWangQueryInvoiceRequest request1 = new BaiWangQueryInvoiceRequest();
                        request1.taxNo = info.TaxNo;
                        request1.orderNo = info.RedOrderNo;
                        var result1 = await _baiWangService.QueryInvoiceAsync(request1,partnerAuthCode.OrgAuthCode);
                        if (result1.success)
                        {
                            if (result1.response.list.Count > 0)
                            {
                                var data_query = result1.response.list[0];
                                //开票状态:00-开票中,01 - 开票完成,02 - 开票失败,03 - 发票已作废,04 - 发票作废中
                                if (data_query.status == "00")
                                {
                                    info.Status = (int)InvoiceStatusEnum.冲红中;
                                }
                                else if (data_query.status == "01")
                                {
                                    info.Status = (int)InvoiceStatusEnum.已冲红;
                                }
                                else if (data_query.status == "02")
                                {
                                    info.Status = (int)InvoiceStatusEnum.冲红失败;
                                }
                                info.RedInvoiceBaseStatus = data_query.invoiceBaseStatus;
                                info.RedDigitInvoiceNo = data_query.digitInvoiceNo;
                                info.Msg = "冲红结果已出";
                                await _invoiceInfoService.Update(info);
                            }
                            return Ok(new { request, result, result1 });


                        }
                    }
                    else
                    {
                        info.Status = (int)InvoiceStatusEnum.红单确认失败;
                        info.Msg = "红单确认失败，请联系客户财务去税务局确认红单，确认后重新提交";
                        await _invoiceInfoService.Update(info);

                    }
                    
                }
            }
            else
            {
                info.Status = (int)InvoiceStatusEnum.冲红失败;
                info.Msg = result.errorResponse.message;
                await _invoiceInfoService.Update(info);
                return Ok(new { request, result });
            }



            return Ok(new { request, result });
        }

        /// <summary>
        /// 红票确认单再次提交
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> InvoiceRedConfirmReset([FromForm] Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ErrorCodeException(-1, "参数错误");
            }
            var info = await _invoiceInfoService.Get(id.Value);
            BaiWangRedConfirmRequest request = new BaiWangRedConfirmRequest()
            {
                taxNo = info.TaxNo,
                operatorType = "1",
                buySelSelector = "0",
                redConfirmSerialNo = info.RedConfirmSerialNo,
                originalDigitInvoiceNo = info.DigitInvoiceNo
            };
            var partnerAuthCode = await _partnerAuthCodeService.Get(t => t.TaxNo == info.TaxNo);
            if (partnerAuthCode == null)
            {
                throw new ErrorCodeException(-1, "开票方未收到邀请，请联系管理员");
            }
            var result = await _baiWangService.IssueRedConfirmAsync(request,partnerAuthCode.OrgAuthCode);
            if (result.success)
            {
                info.Status = (int)InvoiceStatusEnum.冲红中;
                info.Msg = "红单已重新提交确认申请";
            }
            else
            {
                info.Status = (int)InvoiceStatusEnum.冲红失败;
                info.Msg = result.errorResponse.subMessage;
            }
            await _invoiceInfoService.Update(info);


            return Ok(new { request, result });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> OpenOrgRelApi([FromForm] string taxNo, [FromForm] string toTaxCode, [FromForm] string creater)
        {
            List<long> relTypes = new List<long>();
            relTypes.Add(7);
            List<long> authTypes = new List<long>();
            authTypes.Add(2);
            //BaiWangOpenOrgRelApiRequest request = new BaiWangOpenOrgRelApiRequest()
            //{
            //    taxCode = taxNo,
            //    toTaxCode = toTaxCode,
            //    relTypes = relTypes,
            //    authTypes = authTypes,
            //    creater = creater,
            //    sourceFlag = 5,
            //    relUrlType = 2,
            //};
            BaiWangOpenOrgRelApiRequest request = new BaiWangOpenOrgRelApiRequest()
            {
                taxCode = "338888888888SMB",
                toTaxCode = "111111111100000002",
                relTypes = relTypes,
                authTypes = authTypes,
                creater = "负责人",
                sourceFlag = 5,
                relUrlType = 2,
            };
            var result = await _baiWangService.OpenOrgRelApiAsync(request);
            return Ok(new {result,request});
        }

        /// <summary>
        /// 百望开放平台授权通知回调，从请求体读取授权结果并写入 FIN_PartnerAuthCode
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> OrgAuthCallback()
        {
            string body;
            using (var reader = new StreamReader(Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }
            if (string.IsNullOrWhiteSpace(body))
            {
                throw new ErrorCodeException(-1, "回调内容为空");
            }

            var fields = ReadOrgAuthFields(body);
            var appKey = fields.GetValueOrDefault("appKey");
            var taxNo = fields.GetValueOrDefault("taxNo");
            var orgAuthCode = fields.GetValueOrDefault("orgAuthCode");
            var opeTime = fields.GetValueOrDefault("opeTime");
            var opeType = fields.GetValueOrDefault("opeType");
            if (string.IsNullOrWhiteSpace(taxNo))
            {
                throw new ErrorCodeException(-1, "回调参数错误"+ fields);
            }
            var model = await _partnerAuthCodeService.Get(t => t.TaxNo == taxNo);
            if (model == null)
            {
                 throw new ErrorCodeException(-1, "回调内容为空");
            }
            model.AppKey = appKey;
            model.OrgAuthCode = orgAuthCode;
            model.OpeTime = ParseOpeTime(opeTime);
            model.OpeType = ParseOpeType(opeType);
            model.Status = (int)PartnerAuthCodeStatusEnum.已通过;
            await _partnerAuthCodeService.Update(model);
            return Ok(new { success = true, message = "成功" });
        }

        
        private static Dictionary<string, string?> ReadOrgAuthFields(string body)
        {
            var fields = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            var text = body.Trim();
            if (text.StartsWith("{") || text.StartsWith("["))
            {
                using var document = JsonDocument.Parse(text);
                CollectOrgAuthFields(document.RootElement, fields);
                return fields;
            }

            foreach (var pair in text.Split('&', StringSplitOptions.RemoveEmptyEntries))
            {
                var index = pair.IndexOf('=');
                if (index <= 0)
                {
                    continue;
                }
                var name = Uri.UnescapeDataString(pair[..index]).Trim();
                var value = Uri.UnescapeDataString(pair[(index + 1)..]);
                if (IsOrgAuthField(name))
                {
                    fields[name] = value;
                }
            }
            return fields;
        }

        private static void CollectOrgAuthFields(JsonElement element, Dictionary<string, string?> fields)
        {
            if (element.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in element.EnumerateArray())
                {
                    CollectOrgAuthFields(item, fields);
                }
                return;
            }
            if (element.ValueKind == JsonValueKind.String)
            {
                var text = element.GetString()?.Trim();
                if (!string.IsNullOrWhiteSpace(text) && text.StartsWith("{"))
                {
                    using var document = JsonDocument.Parse(text);
                    CollectOrgAuthFields(document.RootElement, fields);
                }
                return;
            }
            if (element.ValueKind != JsonValueKind.Object)
            {
                return;
            }
            foreach (var property in element.EnumerateObject())
            {
                if (IsOrgAuthField(property.Name))
                {
                    fields.TryAdd(property.Name, ReadJsonValue(property.Value));
                    continue;
                }
                if (property.Value.ValueKind is JsonValueKind.Object or JsonValueKind.Array or JsonValueKind.String)
                {
                    CollectOrgAuthFields(property.Value, fields);
                }
            }
        }

        private static bool IsOrgAuthField(string name)
        {
            return name.Equals("appKey", StringComparison.OrdinalIgnoreCase)
                || name.Equals("taxNo", StringComparison.OrdinalIgnoreCase)
                || name.Equals("orgAuthCode", StringComparison.OrdinalIgnoreCase)
                || name.Equals("opeTime", StringComparison.OrdinalIgnoreCase)
                || name.Equals("opeType", StringComparison.OrdinalIgnoreCase);
        }

        private static string? ReadJsonValue(JsonElement value)
        {
            return value.ValueKind switch
            {
                JsonValueKind.String => value.GetString(),
                JsonValueKind.Number => value.GetRawText(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                _ => null
            };
        }

        private static DateTime? ParseOpeTime(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            value = value.Trim();
            var formats = new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-ddTHH:mm:ss", "yyyyMMddHHmmss", "yyyy-MM-dd" };
            if (DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var exact))
            {
                return exact;
            }
            if (DateTime.TryParse(value, out var time))
            {
                return time;
            }
            if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var unix))
            {
                return unix > 9999999999
                    ? DateTimeOffset.FromUnixTimeMilliseconds(unix).LocalDateTime
                    : DateTimeOffset.FromUnixTimeSeconds(unix).LocalDateTime;
            }
            return null;
        }

        private static int? ParseOpeType(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            return int.TryParse(value.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var type) ? type : null;
        }

       
    }

    



}
