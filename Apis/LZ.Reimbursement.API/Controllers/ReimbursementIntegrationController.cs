using LZ.Core.Attributes;
using LZ.Finance.Service.Interface;
using LZ.Model.Dtos.Reimbursement;
using LZ.Model.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LZ.Reimbursement.API.Controllers;

public sealed class ReimbursementIntegrationController : ReimbursementControllerBase
{
    private readonly IReimbursementWriteService _write;

    /// <summary>注入申请写入服务。</summary>
    /// <param name="write">确认打款服务。</param>
    public ReimbursementIntegrationController(IReimbursementWriteService write)
    {
        _write = write;
    }

    /// <summary>由分后台财务人员确认已出款并记录银行流水。</summary>
    /// <param name="applyGuid">待打款申请 GUID。</param>
    /// <param name="request">幂等键、银行流水号及可选回单地址。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>打款单信息。</returns>
    [HttpPost]
    //[Authorize(AuthenticationSchemes = "Authorization-Token")]
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

    /// <summary>保留打款接口原有的错误状态码。</summary>
    /// <param name="status">HTTP 状态码。</param>
    /// <param name="message">错误说明。</param>
    /// <returns>业务错误响应。</returns>
    private IActionResult Error(int status, string message) => new JsonResult(new ApiResponse<object>
    {
        Code = status, Message = message, TraceId = HttpContext.TraceIdentifier
    }) { StatusCode = status };
}
