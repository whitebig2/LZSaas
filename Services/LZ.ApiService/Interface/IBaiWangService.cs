using LZ.Model.BaiWangModel;
using LZ.Model.BaiWangModel.Taxs;

namespace LZ.ApiService.Interface
{
    public interface IBaiWangService
    {
        /// <summary>
        /// 通过 POST 请求百望 OAuth2 接口获取 AccessToken（联调/测试）
        /// </summary>
        /// <param name="cacheToken">是否将 token 写入 Redis 缓存</param>
        Task<BaiWangTokenResponse> GetTokenAsync(bool cacheToken = false);

        /// <summary>
        /// 根据发票查验要素向百望云核验真伪，返回完整票面数据。
        /// </summary>
        Task<BaiWangResponse<BaiWangInvoiceValidationResponse>> VerifyInvoiceAuthenticityAsync(BaiWangInvoiceValidationRequest request);

        /// <summary>
        /// 创建开票申请
        /// </summary>
        Task<BaiWangResponse<BaiWangAddPreInvoiceResponse>> AddPreInvoiceAsync(BaiWangAddPreInvoiceRequest request);

        /// <summary>
        /// 查询开票申请
        /// </summary>
        Task<BaiWangResponse<BaiWangQueryPreInvoiceResponse>> QueryPreInvoiceAsync(BaiWangQueryPreInvoiceRequest request);

        /// <summary>
        /// 查询发票
        /// </summary>
        Task<BaiWangResponse<BaiWangQueryInvoiceResponse>> QueryInvoiceAsync(BaiWangQueryInvoiceRequest request, string authcode);

        /// <summary>
        /// 开具发票
        /// </summary>
        Task<BaiWangResponse<BaiWangIssueInvoiceResponse>> IssueInvoiceAsync(BaiWangIssueInvoiceRequest request,string authcode);

        /// <summary>
        /// 快捷冲红
        /// </summary>
        Task<BaiWangResponse<BaiWangFastRedResponse>> IssueFastRedAsync(BaiWangFastRedRequest request, string authcode);

        /// <summary>
        /// 红票确认单
        /// </summary>
        Task<BaiWangResponse<BaiWangRedConfirmResponse>> IssueRedConfirmAsync(BaiWangRedConfirmRequest request, string authcode);

        /// <summary>
        /// 企业邀请
        /// </summary>
        Task<BaiWangResponse<List<BaiWangOpenOrgRelApi>>> OpenOrgRelApiAsync(BaiWangOpenOrgRelApiRequest request);

        /// <summary>
        /// 个人养老金下载
        /// </summary>
        Task<BaiWangResponse<PersonalPensionDownloadResponse>> PersonalPensionDownloadAsync(PersonalPensionDownloadRequest request);

        /// <summary>
        /// 个税收入明细删除
        /// </summary>
        Task<BaiWangResponse<PersonalTaxIncomeFormDeleteResponse>> PersonalTaxIncomeFormDeleteAsync(PersonalTaxIncomeFormDeleteRequest request);

        /// <summary>
        /// 同步客户信息
        /// </summary>
        Task<BaiWangResponse<PersonalSyncCustomerInfoResponse>> PersonalSyncCustomerInfoAsync(PersonalSyncCustomerInfoRequest request);

        /// <summary>
        /// 个税申报
        /// </summary>
        Task<BaiWangResponse<PersonalDeclareResponse>> PersonalDeclareAsync(PersonalDeclareRequest request);

        /// <summary>
        /// 同步客户账号信息
        /// </summary>
        Task<BaiWangResponse<PersonalSyncCustomerAccountInfoResponse>> PersonalSyncCustomerAccountInfoAsync(PersonalSyncCustomerAccountInfoRequest request);

        /// <summary>
        /// 三方协议缴款
        /// </summary>
        Task<BaiWangResponse<PersonalTripleAgreementPaymentResponse>> PersonalTripleAgreementPaymentAsync(PersonalTripleAgreementPaymentRequest request);

        /// <summary>
        /// 三方协议下载
        /// </summary>
        Task<BaiWangResponse<PersonalTripleAgreementDownloadResponse>> PersonalTripleAgreementDownloadAsync(PersonalTripleAgreementDownloadRequest request);

        /// <summary>
        /// 完税证明开具
        /// </summary>
        Task<BaiWangResponse<PersonalCertificateIssueResponse>> PersonalCertificateIssueAsync(PersonalCertificateIssueRequest request);

        /// <summary>
        /// 专项附加扣除下载
        /// </summary>
        Task<BaiWangResponse<PersonalAdditionDownloadResponse>> PersonalAdditionDownloadAsync(PersonalAdditionDownloadRequest request);

        /// <summary>
        /// 个税收入明细查询
        /// </summary>
        Task<BaiWangResponse<PersonalTaxIncomeFormQueryResponse>> PersonalTaxIncomeFormQueryAsync(PersonalTaxIncomeFormQueryRequest request);

        /// <summary>
        /// 个税收入明细保存
        /// </summary>
        Task<BaiWangResponse<PersonalTaxIncomeFormSaveResponse>> PersonalTaxIncomeFormSaveAsync(PersonalTaxIncomeFormSaveRequest request);

        /// <summary>
        /// 人员信息保存
        /// </summary>
        Task<BaiWangResponse<PersonalUserSaveResponse>> PersonalUserSaveAsync(PersonalUserSaveRequest request);

        /// <summary>
        /// 开始更正
        /// </summary>
        Task<BaiWangResponse<PersonalStartCorrectionResponse>> PersonalStartCorrectionAsync(PersonalStartCorrectionRequest request);

        /// <summary>
        /// 作废申报
        /// </summary>
        Task<BaiWangResponse<PersonalInvalidDeclareResponse>> PersonalInvalidDeclareAsync(PersonalInvalidDeclareRequest request);

        /// <summary>
        /// 取消更正
        /// </summary>
        Task<BaiWangResponse<PersonalCancelCorrectionResponse>> PersonalCancelCorrectionAsync(PersonalCancelCorrectionRequest request);

        /// <summary>
        /// 下载累计数据
        /// </summary>
        Task<BaiWangResponse<PersonalDownloadCumulativeDataResponse>> PersonalDownloadCumulativeDataAsync(PersonalDownloadCumulativeDataRequest request);

        /// <summary>
        /// 复制上月工资薪金
        /// </summary>
        Task<BaiWangResponse<PersonalCopyLastMonthSalaryWagesResponse>> PersonalCopyLastMonthSalaryWagesAsync(PersonalCopyLastMonthSalaryWagesRequest request);

        /// <summary>
        /// 人员列表查询
        /// </summary>
        Task<BaiWangResponse<InvestorUserListQueryResponse>> InvestorUserListQueryAsync(InvestorUserListQueryRequest request);

        /// <summary>
        /// 商业健康险编辑
        /// </summary>
        Task<BaiWangResponse<InvestorCommercialHealthEditResponse>> InvestorCommercialHealthEditAsync(InvestorCommercialHealthEditRequest request);

        /// <summary>
        /// 商业健康险查询
        /// </summary>
        Task<BaiWangResponse<InvestorCommercialHealthQueryResponse>> InvestorCommercialHealthQueryAsync(InvestorCommercialHealthQueryRequest request);

        /// <summary>
        /// 减免事项查询
        /// </summary>
        Task<BaiWangResponse<InvestorReduceQueryResponse>> InvestorReduceQueryAsync(InvestorReduceQueryRequest request);

        /// <summary>
        /// 公益慈善捐赠编辑
        /// </summary>
        Task<BaiWangResponse<InvestorCharityDonationEditResponse>> InvestorCharityDonationEditAsync(InvestorCharityDonationEditRequest request);

        /// <summary>
        /// 地区基金下载
        /// </summary>
        Task<BaiWangResponse<InvestorRegionalFundDownloadResponse>> InvestorRegionalFundDownloadAsync(InvestorRegionalFundDownloadRequest request);

        /// <summary>
        /// 企业信息查询
        /// </summary>
        Task<BaiWangResponse<InvestorEnterpriseInfoQueryResponse>> InvestorEnterpriseInfoQueryAsync(InvestorEnterpriseInfoQueryRequest request);

        /// <summary>
        /// 减免事项编辑
        /// </summary>
        Task<BaiWangResponse<InvestorReduceEditResponse>> InvestorReduceEditAsync(InvestorReduceEditRequest request);

        /// <summary>
        /// 公益慈善捐赠查询
        /// </summary>
        Task<BaiWangResponse<InvestorCharityDonationQueryResponse>> InvestorCharityDonationQueryAsync(InvestorCharityDonationQueryRequest request);
    }
}
