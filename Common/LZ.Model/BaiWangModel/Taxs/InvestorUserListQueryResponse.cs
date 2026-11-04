namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 人员列表查询响应
    /// </summary>
    public class InvestorUserListQueryResponse
    {
        /// <summary>
        /// 人员列表，非必填
        /// </summary>
        public List<InvestorUserListQueryListItem> List { get; set; } = new();

        /// <summary>
        /// 记录数，长度 19，非必填
        /// </summary>
        public long TotalCount { get; set; }

        /// <summary>
        /// 页码，长度 19，非必填
        /// </summary>
        public long PageNo { get; set; }

        /// <summary>
        /// 每页条数，长度 19，非必填
        /// </summary>
        public long PageSize { get; set; }
    }

    /// <summary>
    /// 人员列表项（CsInvestorUser_list_queryListItem）
    /// </summary>
    public class InvestorUserListQueryListItem
    {
        /// <summary>
        /// 客户税号，长度 18，非必填
        /// </summary>
        public string CustomerTaxNo { get; set; } = string.Empty;

        /// <summary>
        /// 姓名，长度 50，非必填
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 证件类型编码，长度 3，非必填
        /// </summary>
        public string IdCardType { get; set; } = string.Empty;

        /// <summary>
        /// 证件号码，长度 18，非必填
        /// </summary>
        public string IdCardNo { get; set; } = string.Empty;

        /// <summary>
        /// 用户唯一标识码，长度 64，非必填
        /// </summary>
        public string StaffId { get; set; } = string.Empty;

        /// <summary>
        /// 其他证件号码，长度 18，非必填
        /// </summary>
        public string OtherIdCardNo { get; set; } = string.Empty;

        /// <summary>
        /// 性别，1-男，2-女，长度 19，非必填
        /// </summary>
        public long Sex { get; set; }

        /// <summary>
        /// 手机号码，长度 11，非必填
        /// </summary>
        public string Mobile { get; set; } = string.Empty;

        /// <summary>
        /// 是否残疾，0-否，1-是，长度 19，非必填
        /// </summary>
        public long IsDisability { get; set; }

        /// <summary>
        /// 残疾证件号码，长度 32，非必填
        /// </summary>
        public string DisabilityCardNo { get; set; } = string.Empty;

        /// <summary>
        /// 是否烈属，0-否，1-是，长度 19，非必填
        /// </summary>
        public long IsMartyrFamilies { get; set; }

        /// <summary>
        /// 烈属证号，长度 32，非必填
        /// </summary>
        public string MartyrFamiliesCardNo { get; set; } = string.Empty;

        /// <summary>
        /// 是否孤老，0-否，1-是，长度 19，非必填
        /// </summary>
        public long IsLonelyOld { get; set; }

        /// <summary>
        /// 部门编号，长度 32，非必填
        /// </summary>
        public string DepartmentNo { get; set; } = string.Empty;

        /// <summary>
        /// 人员状态，0-在职，1-离职，长度 19，非必填
        /// </summary>
        public long Status { get; set; }

        /// <summary>
        /// 受雇从业类型，见受雇从业类型，长度 19，非必填
        /// </summary>
        public long EmploymentType { get; set; }

        /// <summary>
        /// 是否扣除减除费用，0-否，1-是，长度 19，非必填
        /// </summary>
        public long IsDeductExpense { get; set; }

        /// <summary>
        /// 报送状态，见报送状态，长度 19，非必填
        /// </summary>
        public long DeclareStatus { get; set; }

        /// <summary>
        /// 报送失败原因，长度 200，非必填
        /// </summary>
        public string DeclareFailReason { get; set; } = string.Empty;

        /// <summary>
        /// 身份验证状态，见身份验证状态，长度 19，非必填
        /// </summary>
        public long VerifyStatus { get; set; }

        /// <summary>
        /// 身份验证失败原因，长度 200，非必填
        /// </summary>
        public string VerifyFailReason { get; set; } = string.Empty;

        /// <summary>
        /// 标签，null 为空，1-不完整，2-不申报，长度 10，非必填
        /// </summary>
        public string Tag { get; set; } = string.Empty;
    }
}
