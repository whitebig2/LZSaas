using System.Security.Cryptography;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Dtos.Reimbursement;
using LZ.Model.Enums;
using LZ.Model.Enums.Reimbusement;
using LZ.ApiService.Interface;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace LZ.Finance.Service.Services;

public sealed class ReimbursementInvoiceService : ServicesBase<FIN_Reimb_InvoiceUpload>,
    IReimbursementInvoiceService, IScopedDependency
{
    private readonly ICurrentUser _user;
    private readonly IAliyunOSSService _oss;
    private readonly IServiceProvider _services;

    /// <summary>接入当前用户、OSS 和可选的发票/字典扩展服务。</summary>
    /// <param name="user">当前登录用户。</param>
    /// <param name="oss">发票文件存储服务。</param>
    /// <param name="services">解析外部发票和字典提供方的服务容器。</param>
    public ReimbursementInvoiceService(ICurrentUser user, IAliyunOSSService oss, IServiceProvider services)
    {
        _user = user;
        _oss = oss;
        _services = services;
    }

    private int CompanyId
    {
        get
        {
            if (!_user.IsAuthenticated || _user.CompanyID <= 0 || _user.UserID <= 0)
                throw new UnauthorizedAccessException("仅当前企业的小程序用户可上传发票");
            return _user.CompanyID;
        }
    }

    /// <summary>保存首次或补传发票，建立上传记录并启动异步识别。</summary>
    /// <param name="request">文件、上传用途和补传申请 GUID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>上传标识和待识别状态。</returns>
    public async Task<InvoiceUploadDto> UploadAsync(InvoiceUploadRequest request,
        CancellationToken cancellationToken = default)
    {
        var companyId = CompanyId;
        if (request?.File is null || request.File.Length == 0 ||
            request.UploadPurpose is not (InvoiceUploadPurpose.Initial or InvoiceUploadPurpose.Supplement))
            throw new ArgumentException("发票文件或用途无效");
        var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
        if (extension is not (".pdf" or ".jpg" or ".jpeg" or ".png"))
            throw new ArgumentException("仅支持 PDF、JPG、JPEG 和 PNG 发票");
        if (request.File.FileName.Length > 255)
            throw new ArgumentException("文件名过长");
        if (request.UploadPurpose == InvoiceUploadPurpose.Supplement)
        {
            if (request.ApplyGuid is not Guid applyGuid)
                throw new ArgumentException("补传发票必须指定申请");
            var apply = await Context.Queryable<FIN_Reimb_Apply>()
                .FirstAsync(x => x.CompanyId == companyId && x.ApplicantId == _user.UserID &&
                                 x.ApplyGuid == applyGuid && x.ApplyType == 2 && x.InvoiceSupplementStatus == 1);
            if (apply is null) throw new InvalidOperationException("申请不处于待补传发票状态");
        }
        else if (request.ApplyGuid.HasValue)
            throw new ArgumentException("首次提交的发票不能预先关联申请");

        var provider = _services.GetService<IReimbursementInvoiceProvider>()
            ?? throw new InvalidOperationException("发票识别服务暂不可用");
        await using var stream = request.File.OpenReadStream();
        using var md5 = MD5.Create();
        var digest = Convert.ToHexString(await md5.ComputeHashAsync(stream, cancellationToken));
        var duplicate = await Context.Queryable<FIN_Reimb_InvoiceUpload>()
            .FirstAsync(x => x.CompanyId == companyId && x.UploadUserId == _user.UserID &&
                             x.FileMd5 == digest && x.IsEffective);
        if (duplicate is not null) throw new InvalidOperationException("再次上传即可");

        // IFormFile 的读取流未必支持 Seek，重新打开以便 OSS 获得完整文件。
        await using var uploadStream = request.File.OpenReadStream();
        var saved = await _oss.UploadFile(uploadStream, request.File.FileName, true, false, "reimbursement/invoices");
        if (saved.Code != 0 || saved.Data is not string fileUrl || string.IsNullOrWhiteSpace(fileUrl))
            throw new InvalidOperationException("发票文件上传失败");
        var upload = new FIN_Reimb_InvoiceUpload
        {
            UploadGuid = Guid.NewGuid(),
            CompanyId = companyId,
            ApplyGuid = request.ApplyGuid,
            ProviderRequestId = Guid.NewGuid().ToString("N"),
            UploadPurpose = (byte)request.UploadPurpose,
            FileUrl = fileUrl,
            FileType = extension.TrimStart('.').ToUpperInvariant(),
            OriginalFileName = request.File.FileName,
            FileMd5 = digest,
            RecognitionStatus = 0,
            VerifyStatus = 0,
            DuplicateCheckStatus = 0,
            IsEffective = false,
            UploadUserId = _user.UserID,
            UploadedAt = DateTime.Now
        };
        await Context.Insertable(upload).IgnoreColumns(x => new { x.Id, x.RowVersion }).ExecuteCommandAsync();
        try
        {
            // ProviderRequestId connects the asynchronous callback to this exact upload.
            await provider.StartRecognitionAsync(companyId, upload.UploadGuid, upload.ProviderRequestId,
                fileUrl, cancellationToken);
        }
        catch
        {
            await Context.Updateable<FIN_Reimb_InvoiceUpload>()
                .SetColumns(x => new FIN_Reimb_InvoiceUpload
                {
                    RecognitionStatus = 2,
                    FailureMessage = "再次上传即可",
                    FailureCode = "PROVIDER_UNAVAILABLE"
                })
                .Where(x => x.CompanyId == companyId && x.UploadGuid == upload.UploadGuid)
                .ExecuteCommandAsync();
            throw;
        }
        return MapUpload(upload);
    }

    /// <summary>验签后处理 OCR 回调，更新发票识别、验真及查重状态。</summary>
    /// <param name="request">解析后的供应商回调。</param>
    /// <param name="signature">请求签名。</param>
    /// <param name="rawBody">原始回调正文，用于验签。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    public async Task HandleCallbackAsync(InvoiceCallbackRequest request, string signature, string rawBody,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var provider = _services.GetService<IReimbursementInvoiceProvider>()
            ?? throw new InvalidOperationException("发票识别服务暂不可用");
        if (!provider.VerifyCallback(signature, rawBody))
            throw new UnauthorizedAccessException("发票回调签名无效");
        if (request is null || request.UploadGuid == Guid.Empty || string.IsNullOrWhiteSpace(request.ProviderRequestId))
            throw new ArgumentException("发票回调标识无效");
        var upload = await Context.Queryable<FIN_Reimb_InvoiceUpload>()
            .FirstAsync(x => x.UploadGuid == request.UploadGuid);
        if (upload is null || upload.ProviderRequestId != request.ProviderRequestId)
            throw new UnauthorizedAccessException("发票回调流水号不匹配");
        if (upload.CallbackAt.HasValue) return;

        var validFields = request.RecognitionSucceeded && request.VerifySucceeded &&
                          !string.IsNullOrWhiteSpace(request.InvoiceNumber) && request.InvoiceNumber.Length <= 50 &&
                          request.TotalAmount > 0 && decimal.Round(request.TotalAmount.Value, 2) == request.TotalAmount;
        var occupied = validFields && await Context.Queryable<FIN_Reimb_InvoicePool>()
            .AnyAsync(x => x.CompanyId == upload.CompanyId && x.InvoiceNumber == request.InvoiceNumber);
        var sameUpload = validFields && await Context.Queryable<FIN_Reimb_InvoiceUpload>()
            .AnyAsync(x => x.CompanyId == upload.CompanyId && x.UploadGuid != upload.UploadGuid &&
                           x.InvoiceNumber == request.InvoiceNumber && x.IsEffective);
        var effective = validFields && !occupied && !sameUpload;
        // Persist only the two allowed OCR fields; raw provider payload is deliberately discarded.
        var changed = await Context.Updateable<FIN_Reimb_InvoiceUpload>()
            .SetColumns(x => new FIN_Reimb_InvoiceUpload
            {
                InvoiceNumber = request.RecognitionSucceeded ? request.InvoiceNumber : null,
                TotalAmount = request.RecognitionSucceeded ? request.TotalAmount : null,
                RecognitionStatus = request.RecognitionSucceeded ? (byte)1 : (byte)2,
                VerifyStatus = request.VerifySucceeded ? (byte)1 : (byte)2,
                DuplicateCheckStatus = occupied || sameUpload ? (byte)2 : validFields ? (byte)1 : (byte)0,
                IsEffective = effective,
                FailureCode = effective ? null : request.FailureCode ?? "INVOICE_INVALID",
                FailureMessage = effective ? null : "再次上传即可",
                CallbackAt = DateTime.Now
            })
            .Where(x => x.CompanyId == upload.CompanyId && x.UploadGuid == upload.UploadGuid &&
                        x.ProviderRequestId == request.ProviderRequestId && x.CallbackAt == null)
            .ExecuteCommandAsync();
        if (changed != 1) throw new InvalidOperationException("发票回调状态已变化，请刷新后重试");
    }

    /// <summary>按企业从字典扩展服务读取申请表单选项。</summary>
    /// <param name="kind">法人主体、成本类型、费用名称或银行。</param>
    /// <param name="keyword">可选搜索词。</param>
    /// <param name="costTypeId">费用名称的成本类型筛选值。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>字典项列表。</returns>
    public async Task<IReadOnlyList<DictItemDto>> GetDictionaryAsync(string kind, string? keyword,
        int? costTypeId, CancellationToken cancellationToken = default)
    {
        var companyId = CompanyId;
        if (kind is not ("legal-entities" or "cost-types" or "expense-names" or "payee-banks"))
            throw new ArgumentException("字典类型无效");
        var provider = _services.GetService<IReimbursementDictionaryProvider>()
            ?? throw new InvalidOperationException("报销字典服务暂不可用");
        return await provider.GetAsync(companyId, kind, keyword, costTypeId, cancellationToken);
    }

    /// <summary>将数据库上传记录转换为小程序可读取的识别状态。</summary>
    /// <param name="upload">发票上传实体。</param>
    /// <returns>上传状态 DTO。</returns>
    private static InvoiceUploadDto MapUpload(FIN_Reimb_InvoiceUpload upload) => new()
    {
        UploadGuid = upload.UploadGuid,
        RecognitionStatus = upload.RecognitionStatus,
        VerifyStatus = upload.VerifyStatus,
        DuplicateCheckStatus = upload.DuplicateCheckStatus,
        IsEffective = upload.IsEffective,
        InvoiceNumber = upload.InvoiceNumber,
        TotalAmount = upload.TotalAmount,
        FailureMessage = upload.FailureMessage
    };
}
