using System.Text;
using System.Text.Json;
using LZ.Core.Attributes;
using LZ.Finance.Service.Interface;
using LZ.Model.Dtos.Reimbursement;
using LZ.Model.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LZ.Reimbursement.API.Controllers;

public sealed class ReimbursementIntegrationController : ReimbursementControllerBase
{
    private readonly IReimbursementInvoiceService _invoices;
    private readonly IReimbursementWriteService _write;

    /// <summary>注入发票回调处理和申请写入服务。</summary>
    /// <param name="invoices">发票回调处理服务。</param>
    /// <param name="write">确认打款服务。</param>
    public ReimbursementIntegrationController(IReimbursementInvoiceService invoices,
        IReimbursementWriteService write)
    {
        _invoices = invoices;
        _write = write;
    }

    /// <summary>接收发票供应商异步回调，验签后更新识别和验真状态。</summary>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>回调处理结果。</returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> InvoiceCallback(CancellationToken cancellationToken)
    {
        try
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var raw = await reader.ReadToEndAsync(cancellationToken);
            var callback = JsonSerializer.Deserialize<InvoiceCallbackRequest>(raw,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (callback is null) return Error(400, "发票回调内容无效");
            await _invoices.HandleCallbackAsync(callback, Request.Headers["X-Invoice-Signature"].ToString(),
                raw, cancellationToken);
            return Ok();
        }
        catch (UnauthorizedAccessException ex) { return Error(401, ex.Message); }
        catch (ArgumentException ex) { return Error(400, ex.Message); }
        catch (InvalidOperationException ex) { return Error(409, ex.Message); }
    }

    /// <summary>由分后台财务人员确认已出款并记录银行流水。</summary>
    /// <param name="applyGuid">待打款申请 GUID。</param>
    /// <param name="request">幂等键、银行流水号及可选回单地址。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>打款单信息。</returns>
    [HttpPost]
    [Authorize(AuthenticationSchemes = "Authorization-Token")]
    [PlatformLimit(PlatformType.MiniAdmin)]
    public async Task<IActionResult> CompletePayment(Guid applyGuid, [FromBody] CompletePaymentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _write.CompletePaymentAsync(applyGuid, request, cancellationToken);
            return ToCamelCase(Ok(result));
        }
        catch (ArgumentException ex) { return Error(400, ex.Message); }
        catch (UnauthorizedAccessException ex) { return Error(403, ex.Message); }
        catch (InvalidOperationException ex) { return Error(409, ex.Message); }
    }

    /// <summary>保留发票回调和打款接口原有的错误状态码。</summary>
    /// <param name="status">HTTP 状态码。</param>
    /// <param name="message">错误说明。</param>
    /// <returns>业务错误响应。</returns>
    private IActionResult Error(int status, string message) => new JsonResult(new ApiResponse<object>
    {
        Code = status, Message = message, TraceId = HttpContext.TraceIdentifier
    }) { StatusCode = status };
}
