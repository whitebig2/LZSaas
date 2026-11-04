namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 人员信息保存响应
    /// </summary>
    public class PersonalUserSaveResponse
    {
        /// <summary>
        /// 姓名，长度 10，非必填
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 失败原因，长度 255，非必填
        /// </summary>
        public string FailReason { get; set; } = string.Empty;

        /// <summary>
        /// 报送状态，0-待报送，1-报送中，2-报送成功，3-报送失败，长度 1，非必填
        /// </summary>
        public string DeclareStatus { get; set; } = string.Empty;

        /// <summary>
        /// 身份验证状态，0-待验证，1-验证通过，2-验证不通过，长度 1，非必填
        /// </summary>
        public string VerifyStatus { get; set; } = string.Empty;

        /// <summary>
        /// 证件类型，长度 20，非必填
        /// </summary>
        public string IdCardType { get; set; } = string.Empty;

        /// <summary>
        /// 证件号码，长度 20，非必填
        /// </summary>
        public string IdCardNo { get; set; } = string.Empty;

        /// <summary>
        /// 个人档案号，长度 20，非必填
        /// </summary>
        public string IndividualFileNo { get; set; } = string.Empty;
    }
}
