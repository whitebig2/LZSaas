namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 同步客户信息响应
    /// </summary>
    public class PersonalSyncCustomerInfoResponse
    {
        /// <summary>
        /// 数电账号是否保存成功，非必填
        /// </summary>
        public bool DigitAccountSaveSuccess { get; set; }

        /// <summary>
        /// 保存失败原因，长度 100，非必填
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
