namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 个税收入明细删除响应
    /// </summary>
    public class PersonalTaxIncomeFormDeleteResponse
    {
        /// <summary>
        /// 删除成功的 uuid 列表，非必填
        /// </summary>
        public List<string> SuccessList { get; set; } = new();

        /// <summary>
        /// 删除失败的列表，非必填
        /// </summary>
        public List<PersonalTaxIncomeFormDeleteFail> FailList { get; set; } = new();
    }

    /// <summary>
    /// 个税收入明细删除失败项（CsPersonalPersonalTaxIncomeFormDeleteFail）
    /// </summary>
    public class PersonalTaxIncomeFormDeleteFail
    {
        /// <summary>
        /// 收入 uuid，长度 36，非必填
        /// </summary>
        public string Uuid { get; set; } = string.Empty;

        /// <summary>
        /// 删除失败原因，长度 100，非必填
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
