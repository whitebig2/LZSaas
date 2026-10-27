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

        public BaiWangController(IBaiWangService baiWangService)
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
        [HttpGet]
        public async Task<IActionResult> IssueInvoice()
        {
            List<BaiWangInvoiceDetail> details = new List<BaiWangInvoiceDetail>();
            BaiWangInvoiceDetail detail = new BaiWangInvoiceDetail()
            {
                goodsName = "咨询服务",
                goodsCode = "3040203000000000000",
                goodsQuantity = "1",
                goodsPrice = "100.00",
                goodsTaxRate = "0.06"
            };
            details.Add(detail);
            BaiWangIssueInvoiceRequest request = new BaiWangIssueInvoiceRequest()
            {
                taxNo = "338888888888SMB",
                orderNo = "INV20260914002",
                orderDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                invoiceTerminalCode = "15888888888",
                invoiceTypeCode = "02",
                invoiceType = "1",
                priceTaxMark = "1",
                buyerName = "测试购方公司",
                drawer = "开票人",
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
            return Ok(result);
        }
    }

    



}
