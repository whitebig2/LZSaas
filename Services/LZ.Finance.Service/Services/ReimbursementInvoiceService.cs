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

/// <summary>处理报销发票上传、三项校验及后续申请可读取的上传状态。</summary>
public sealed class ReimbursementInvoiceService : ServicesBase<FIN_Reimb_InvoiceUpload>,
    IReimbursementInvoiceService, IScopedDependency
{
    private readonly ICurrentUser _user;
    private readonly IAliyunOSSService _oss;
    private readonly IServiceProvider _services;
    private readonly IReimbursementInvoiceRecognitionClient _recognition;

    /// <summary>接入当前用户、OSS 和可选的发票/字典扩展服务。</summary>
    /// <param name="user">当前登录用户。</param>
    /// <param name="oss">发票文件存储服务。</param>
    /// <param name="services">解析外部发票和字典提供方的服务容器。</param>
    /// <param name="recognition">报销专用的 OCR 与百望云验真适配器。</param>
    public ReimbursementInvoiceService(ICurrentUser user, IAliyunOSSService oss, IServiceProvider services,
        IReimbursementInvoiceRecognitionClient recognition)
    {
        _user = user;
        _oss = oss;
        _services = services;
        _recognition = recognition;
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

    /// <summary>保存首次或补传发票，同步完成识别、验真与查重。</summary>
    /// <param name="request">文件、上传用途和补传申请 GUID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>InvoiceUploadDto，包含上传 GUID、识别/验真/查重状态、有效性和票面金额。</returns>
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
        await using var uploadStream = request.File.OpenReadStream();
        using var md5 = MD5.Create();
        var digest = Convert.ToHexString(await md5.ComputeHashAsync(uploadStream, cancellationToken));
        // OSS 上传服务会将流定位到开头；同一文件流供摘要计算和原件上传使用。
        uploadStream.Position = 0;
        var saved = await _oss.UploadFile(uploadStream, request.File.FileName, true, false, "reimbursement/invoices");
        if (saved.Code != 0 || saved.Data is not string fileUrl || string.IsNullOrWhiteSpace(fileUrl))
            throw new InvalidOperationException("发票文件上传失败");
        // 当前为同步识别；旧异步插件使用的 ProviderRequestId/CallbackAt 留空。
        var upload = new FIN_Reimb_InvoiceUpload
        {
            UploadGuid = Guid.NewGuid(),
            CompanyId = companyId,
            ApplyGuid = request.ApplyGuid,
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
        ReimbursementVerifiedInvoice verified;
        try
        {
            var recognized = await _recognition.RecognizeAsync(request.File, extension, fileUrl, cancellationToken);
            upload.InvoiceNumber = recognized.InvoiceNumber;
            upload.RecognitionStatus = 1;
            verified = await _recognition.VerifyAsync(recognized, cancellationToken);
        }
        catch (Exception ex)
        {
            upload.RecognitionStatus = upload.RecognitionStatus == 1 ? (byte)1 : (byte)2;
            upload.VerifyStatus = upload.RecognitionStatus == 1 ? (byte)2 : (byte)0;
            upload.IsEffective = false;
            upload.FailureCode = ex is ReimbursementInvoiceAuthorizationException
                ? "VERIFY_AUTHORIZATION_FAILED"
                : upload.RecognitionStatus == 1 ? "VERIFY_FAILED" : "RECOGNITION_FAILED";
            upload.FailureMessage = ex is ReimbursementInvoiceAuthorizationException
                ? "百望云未授权查验该购方税号，请联系管理员" : "再次上传即可";
            var changed = await Context.Updateable<FIN_Reimb_InvoiceUpload>()
                .SetColumns(x => new FIN_Reimb_InvoiceUpload
                {
                    InvoiceNumber = upload.InvoiceNumber,
                    RecognitionStatus = upload.RecognitionStatus,
                    VerifyStatus = upload.VerifyStatus,
                    IsEffective = false,
                    FailureMessage = upload.FailureMessage,
                    FailureCode = upload.FailureCode
                })
                .Where(x => x.CompanyId == companyId && x.UploadGuid == upload.UploadGuid &&
                            x.RecognitionStatus == 0 && x.VerifyStatus == 0 && x.DuplicateCheckStatus == 0)
                .ExecuteCommandAsync();
            if (changed != 1) throw new InvalidOperationException("发票上传状态已变化，请刷新后重试");
            if (ex is OperationCanceledException or
                InvalidOperationException { Message: "PDF 包含多张不同发票，请逐张上传" })
                throw;
            return MapUpload(upload);
        }

        upload.TotalAmount = verified.TotalAmount;
        upload.VerifyStatus = 1;
        // 发票池与上传表按固定顺序加表锁，与申请关联时的入池锁一致。
        await Context.Ado.BeginTranAsync();
        try
        {
            var occupied = await Context.Queryable<FIN_Reimb_InvoicePool>()
                .With("WITH(TABLOCKX,HOLDLOCK)")
                .AnyAsync(x => x.CompanyId == companyId && x.InvoiceNumber == verified.InvoiceNumber);
            var duplicate = await Context.Queryable<FIN_Reimb_InvoiceUpload>()
                .With("WITH(TABLOCKX,HOLDLOCK)")
                .AnyAsync(x => x.CompanyId == companyId && x.UploadGuid != upload.UploadGuid &&
                               x.InvoiceNumber == verified.InvoiceNumber && x.IsEffective);
            upload.DuplicateCheckStatus = occupied || duplicate ? (byte)2 : (byte)1;
            upload.IsEffective = !occupied && !duplicate;
            upload.FailureCode = upload.IsEffective ? null! : "INVOICE_DUPLICATE";
            upload.FailureMessage = upload.IsEffective ? null! : "再次上传即可";
            var changed = await Context.Updateable<FIN_Reimb_InvoiceUpload>()
                .SetColumns(x => new FIN_Reimb_InvoiceUpload
                {
                    InvoiceNumber = verified.InvoiceNumber,
                    TotalAmount = verified.TotalAmount,
                    RecognitionStatus = 1,
                    VerifyStatus = 1,
                    DuplicateCheckStatus = upload.DuplicateCheckStatus,
                    IsEffective = upload.IsEffective,
                    FailureCode = upload.FailureCode,
                    FailureMessage = upload.FailureMessage
                })
                .Where(x => x.CompanyId == companyId && x.UploadGuid == upload.UploadGuid &&
                            x.RecognitionStatus == 0 && x.VerifyStatus == 0 && x.DuplicateCheckStatus == 0)
                .ExecuteCommandAsync();
            if (changed != 1) throw new InvalidOperationException("发票上传状态已变化，请刷新后重试");
            await Context.Ado.CommitTranAsync();
        }
        catch
        {
            await Context.Ado.RollbackTranAsync();
            throw;
        }
        return MapUpload(upload);
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
