using LZ.Model.BaiWangModel;

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
        Task<BaiWangResponse<BaiWangQueryInvoiceResponse>> QueryInvoiceAsync(BaiWangQueryInvoiceRequest request);

        /// <summary>
        /// 开具发票
        /// </summary>
        Task<BaiWangResponse<BaiWangIssueInvoiceResponse>> IssueInvoiceAsync(BaiWangIssueInvoiceRequest request);
    }
}
