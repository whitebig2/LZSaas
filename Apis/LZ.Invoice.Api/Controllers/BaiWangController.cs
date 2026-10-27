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
using System.Text.Json;

namespace LZ.Invoice.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BaiWangController : InvoiceControllerBase
    {
        private readonly IBaiWangService _baiWangService;
        private readonly IInvoiceInfoService _invoiceInfoService;
        private readonly BaiWangOption _option;

        public BaiWangController(IBaiWangService baiWangService, IInvoiceInfoService invoiceInfoService, IOptions<BaiWangOption> option)
        {
            _baiWangService = baiWangService;
            _invoiceInfoService = invoiceInfoService;
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
            var result = await _baiWangService.QueryInvoiceAsync(request);
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
            decimal goodsTotalPrice = Math.Round(goodsTotalPriceTax / (1M + goodsTaxRate), 2) ;//不含税金额
            decimal goodsTotalTax = Math.Round(goodsTotalPrice * goodsTaxRate, 2);
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
                invoiceTerminalCode = info.InvoiceTerminalCode,
                invoiceTypeCode = info.InvoiceTypeCode,
                invoiceType = "1",//1-蓝票,2-红票
                priceTaxMark = "1",//单价含税标志:0-不含税,1 - 含税
                buyerName = info.BuyerName,
                buyerTaxNo = info.BuyerTaxNo,
                callBackUrl = _option.HostUrl + "/api/invoice/BaiWang/InvoiceCallback",
                invoiceDetailList = details
            };
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
                if(request.invoiceTerminalCode == "01")
                {
                    balance.balanceType = "10";
                }
                else if (request.invoiceTerminalCode == "02")
                {
                    balance.balanceType = "11";
                }
                balance.balanceTotalAmount = detail.goodsTotalPriceTax;
                decimal deductAmount = info.DeductibleAmount.HasValue ? info.DeductibleAmount.Value : 0M;
                balance.balanceDeductAmount = deductAmount.ToString();
                balanceInfoList.Add(balance);
                request.balanceInfoList = balanceInfoList;
            }



            

            var result = await _baiWangService.IssueInvoiceAsync(request);
            if (result.success)
            {
                info.Status = (int)InvoiceStatusEnum.开具中;
                info.SerialNo = result.response.serialNo;
                info.Msg = "开具申请成功";
            }
            else
            {
                info.Status = (int)InvoiceStatusEnum.开具失败;
                info.Msg = result.errorResponse.message;
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
            var result = await _baiWangService.QueryInvoiceAsync(request);
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
            var result = await _baiWangService.IssueFastRedAsync(request);
            if (result.success)
            {
                info.RedConfirmSerialNo = result.response.redConfirmSerialNo;
                info.Status = (int)InvoiceStatusEnum.冲红中;
                info.Msg = "冲红申请成功";
            }
            else
            {
                info.Status = (int)InvoiceStatusEnum.冲红失败;
                info.Msg = result.errorResponse.message;
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
            var result = await _baiWangService.IssueRedConfirmAsync(request);
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
                        var result1 = await _baiWangService.QueryInvoiceAsync(request1);
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
            var result = await _baiWangService.IssueRedConfirmAsync(request);
            if (result.success)
            {
                info.Status = (int)InvoiceStatusEnum.冲红中;
                info.Msg = "红单已重新提交确认申请";
            }
            else
            {
                info.Status = (int)InvoiceStatusEnum.冲红失败;
                info.Msg = result.errorResponse.message;
            }
            await _invoiceInfoService.Update(info);


            return Ok(new { request, result });
        }


        /// <summary>
        /// 接收百望开票结果回调，按 orderno 回写开票状态
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> InvoiceCallback()
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

            using var document = JsonDocument.Parse(body);
            var items = new List<JsonElement>();
            CollectInvoiceCallbackItems(document.RootElement, items);
            if (items.Count == 0)
            {
                throw new ErrorCodeException(-1, "回调缺少orderno");
            }

            foreach (var item in items)
            {
                var orderNo = ReadCallbackString(item, "orderNo");
                if (string.IsNullOrWhiteSpace(orderNo))
                {
                    throw new ErrorCodeException(-1, "回调缺少orderno");
                }
                orderNo = orderNo.Trim();
                var infos = await _invoiceInfoService.List(f => f.OrderNo == orderNo || f.RedOrderNo == orderNo);
                var info = infos.FirstOrDefault();
                if (info == null)
                {
                    throw new ErrorCodeException(-1, "未找到对应开票数据");
                }

                var isRed = info.RedOrderNo == orderNo;
                var status = ReadCallbackString(item, "status");
                var invoiceType = ReadCallbackString(item, "invoiceType");
                var success = ReadCallbackBool(item, "success");
                if (!TryApplyInvoiceStatus(info, status, invoiceType, success, isRed))
                {
                    throw new ErrorCodeException(-1, "无法识别开票结果");
                }

                var digitInvoiceNo = ReadCallbackString(item, "digitInvoiceNo");
                if (!string.IsNullOrWhiteSpace(digitInvoiceNo))
                {
                    info.DigitInvoiceNo = digitInvoiceNo;
                }
                info.EditTime = DateTime.Now;
                info.Msg = "开具结果已出";
                await _invoiceInfoService.Update(info);
            }

            return Ok(new { success = true, message = "成功" });
        }

        private static void CollectInvoiceCallbackItems(JsonElement element, List<JsonElement> items)
        {
            if (element.ValueKind == JsonValueKind.Array)
            {
                foreach (var child in element.EnumerateArray())
                {
                    CollectInvoiceCallbackItems(child, items);
                }
                return;
            }
            if (element.ValueKind != JsonValueKind.Object)
            {
                return;
            }
            if (!string.IsNullOrWhiteSpace(ReadCallbackString(element, "orderNo")))
            {
                items.Add(element);
                return;
            }
            foreach (var name in new[] { "data", "response", "list" })
            {
                if (TryGetCallbackProperty(element, name, out var child))
                {
                    CollectInvoiceCallbackItems(child, items);
                }
            }
        }

        private static bool TryApplyInvoiceStatus(FIN_InvoiceInfo info, string? status, string? invoiceType, bool? success, bool isRed)
        {
            var type = string.IsNullOrWhiteSpace(invoiceType) ? (isRed ? "2" : "1") : invoiceType.Trim();
            var isBlue = type == "1";
            status = status?.Trim();
            if (status == "1")
            {
                status = "01";
            }
            else if (status == "2")
            {
                status = "02";
            }
            else if (status == "3")
            {
                status = "03";
            }

            // 开票状态:00-开票中,01-开票完成,02-开票失败,03-发票已作废
            if (status == "01" || (success == true && string.IsNullOrEmpty(status)))
            {
                info.Status = isBlue ? (int)InvoiceStatusEnum.已开具 : (int)InvoiceStatusEnum.已冲红;
                return true;
            }
            if (status == "02" || (success == false && string.IsNullOrEmpty(status)))
            {
                info.Status = isBlue ? (int)InvoiceStatusEnum.开具失败 : (int)InvoiceStatusEnum.冲红失败;
                return true;
            }
            if (status == "00")
            {
                info.Status = isBlue ? (int)InvoiceStatusEnum.开具中 : (int)InvoiceStatusEnum.冲红中;
                return true;
            }
            if (status == "03")
            {
                info.Status = (int)InvoiceStatusEnum.已作废;
                return true;
            }
            return false;
        }

        private static string? ReadCallbackString(JsonElement element, string name)
        {
            if (!TryGetCallbackProperty(element, name, out var value))
            {
                return null;
            }
            return value.ValueKind switch
            {
                JsonValueKind.String => value.GetString(),
                JsonValueKind.Number => value.GetRawText(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                _ => null
            };
        }

        private static bool? ReadCallbackBool(JsonElement element, string name)
        {
            var text = ReadCallbackString(element, name);
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }
            if (bool.TryParse(text, out var result))
            {
                return result;
            }
            if (text == "1")
            {
                return true;
            }
            if (text == "0")
            {
                return false;
            }
            return null;
        }

        private static bool TryGetCallbackProperty(JsonElement element, string name, out JsonElement value)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in element.EnumerateObject())
                {
                    if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
                    {
                        value = property.Value;
                        return true;
                    }
                }
            }
            value = default;
            return false;
        }
    }

    



}
