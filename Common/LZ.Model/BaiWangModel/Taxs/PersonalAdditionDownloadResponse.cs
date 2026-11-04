namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 专项附加扣除下载响应
    /// </summary>
    public class PersonalAdditionDownloadResponse
    {
        /// <summary>
        /// 执行结果，非必填
        /// </summary>
        public PersonalAdditionDownloadResult Result { get; set; } = new();
    }

    /// <summary>
    /// 专项附加扣除下载执行结果
    /// </summary>
    public class PersonalAdditionDownloadResult
    {
        /// <summary>
        /// 专项附加扣除明细，非必填
        /// </summary>
        public List<PersonalAdditionDownloadSpecialAdditionItem> SpecialAdditionItems { get; set; } = new();

        /// <summary>
        /// 人员维度错误信息，非必填
        /// </summary>
        public List<PersonalAdditionDownloadPersonErrorInfo> PersonErrorInfoList { get; set; } = new();
    }

    /// <summary>
    /// 专项附加扣除明细（CsPersonalAdditionDownloadObjectType）
    /// </summary>
    public class PersonalAdditionDownloadSpecialAdditionItem
    {
        /// <summary>
        /// 证件号码，长度 18，非必填
        /// </summary>
        public string IdCardNo { get; set; } = string.Empty;

        /// <summary>
        /// 内层自然人档案号，长度 20，非必填
        /// </summary>
        public string Zrrdah { get; set; } = string.Empty;

        /// <summary>
        /// 子女教育，非必填
        /// </summary>
        public List<PersonalAdditionDownloadChildEducation> ChildEducations { get; set; } = new();

        /// <summary>
        /// 继续教育，类型为学历继续教育，非必填
        /// </summary>
        public List<PersonalAdditionDownloadContinueEducation> ContinueEducations { get; set; } = new();

        /// <summary>
        /// 继续教育，类型为职业资格继续教育，非必填
        /// </summary>
        public List<PersonalAdditionDownloadContinueEducationProfessional> ContinueEducationProfessionals { get; set; } = new();

        /// <summary>
        /// 住房贷款，非必填
        /// </summary>
        public PersonalAdditionDownloadHouseLoan HouseLoan { get; set; } = new();

        /// <summary>
        /// 住房租金，非必填
        /// </summary>
        public List<PersonalAdditionDownloadRent> HouseRent { get; set; } = new();

        /// <summary>
        /// 赡养老人，非必填
        /// </summary>
        public PersonalAdditionDownloadCareOld CareOld { get; set; } = new();

        /// <summary>
        /// 婴幼儿照护，非必填
        /// </summary>
        public List<PersonalAdditionDownloadCareBaby> CareBabies { get; set; } = new();
    }

    /// <summary>
    /// 子女教育（CsPersonalAdditionDownloadChildEducation）
    /// </summary>
    public class PersonalAdditionDownloadChildEducation
    {
        /// <summary>
        /// 证件号码，长度 18，非必填
        /// </summary>
        public string IdCardNo { get; set; } = string.Empty;

        /// <summary>
        /// 姓名，长度 50，非必填
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 生日，长度 20，非必填
        /// </summary>
        public string Birthday { get; set; } = string.Empty;

        /// <summary>
        /// 身份验证状态，0-待验证，1-验证通过，2-验证不通过，长度 1，非必填
        /// </summary>
        public string VerifyStatus { get; set; } = string.Empty;

        /// <summary>
        /// 当前受教育阶段，长度 20，非必填
        /// </summary>
        public string CurrentEducationStage { get; set; } = string.Empty;

        /// <summary>
        /// 受教育日期起，长度 20，非必填
        /// </summary>
        public string EducationBeginDate { get; set; } = string.Empty;

        /// <summary>
        /// 受教育日期止，长度 1，非必填
        /// </summary>
        public string EducationEndDate { get; set; } = string.Empty;

        /// <summary>
        /// 扣除有效期起，长度 20，非必填
        /// </summary>
        public string DeductionBeginDate { get; set; } = string.Empty;

        /// <summary>
        /// 扣除有效期止，长度 10，非必填
        /// </summary>
        public string DeductionEndDate { get; set; } = string.Empty;

        /// <summary>
        /// 本人扣除比例，长度 10，非必填
        /// </summary>
        public string DeductionPercentage { get; set; } = string.Empty;

        /// <summary>
        /// 更新时间，长度 10，非必填
        /// </summary>
        public string UpdateTime { get; set; } = string.Empty;
    }

    /// <summary>
    /// 学历继续教育（CsPersonalAdditionDownloadContinueEducation）
    /// </summary>
    public class PersonalAdditionDownloadContinueEducation
    {
        /// <summary>
        /// 教育阶段，长度 1，非必填
        /// </summary>
        public string EducationStage { get; set; } = string.Empty;

        /// <summary>
        /// 受教育日期起，长度 20，非必填
        /// </summary>
        public string EducationBeginDate { get; set; } = string.Empty;

        /// <summary>
        /// 受教育日期止，长度 20，非必填
        /// </summary>
        public string EducationEndDate { get; set; } = string.Empty;

        /// <summary>
        /// 扣除有效期起，长度 20，非必填
        /// </summary>
        public string DeductionBeginDate { get; set; } = string.Empty;

        /// <summary>
        /// 扣除有效期止，长度 20，非必填
        /// </summary>
        public string DeductionEndDate { get; set; } = string.Empty;

        /// <summary>
        /// 更新时间，长度 20，非必填
        /// </summary>
        public string UpdateTime { get; set; } = string.Empty;
    }

    /// <summary>
    /// 职业资格继续教育（CsPersonalAdditionDownloadContinueEducationProfessional）
    /// </summary>
    public class PersonalAdditionDownloadContinueEducationProfessional
    {
        /// <summary>
        /// 继续教育类型，长度 20，非必填
        /// </summary>
        public string ContinueEducationType { get; set; } = string.Empty;

        /// <summary>
        /// 证书名称，长度 20，非必填
        /// </summary>
        public string LicenseDate { get; set; } = string.Empty;

        /// <summary>
        /// 证书编号，长度 20，非必填
        /// </summary>
        public string DeductionBeginDate { get; set; } = string.Empty;

        /// <summary>
        /// 扣除有效期起，长度 20，非必填
        /// </summary>
        public string DeductionEndDate { get; set; } = string.Empty;

        /// <summary>
        /// 扣除有效期止，长度 20，非必填
        /// </summary>
        public string UpdateTime { get; set; } = string.Empty;
    }

    /// <summary>
    /// 住房贷款
    /// </summary>
    public class PersonalAdditionDownloadHouseLoan
    {
        /// <summary>
        /// 房屋地址，长度 50，非必填
        /// </summary>
        public string HouseAddress { get; set; } = string.Empty;

        /// <summary>
        /// 证书类型，长度 50，非必填
        /// </summary>
        public string LicenseType { get; set; } = string.Empty;

        /// <summary>
        /// 房屋证书号，长度 50，非必填
        /// </summary>
        public string HouseLicenseNo { get; set; } = string.Empty;

        /// <summary>
        /// 本人扣除比例，长度 50，非必填
        /// </summary>
        public string DeductionPercentage { get; set; } = string.Empty;

        /// <summary>
        /// 更新日期，长度 50，非必填
        /// </summary>
        public string UpdateTime { get; set; } = string.Empty;

        /// <summary>
        /// 贷款明细，非必填
        /// </summary>
        public List<PersonalAdditionDownloadHouseLoanItem> HouseLoanItems { get; set; } = new();
    }

    /// <summary>
    /// 住房贷款明细（CsPersonalAdditionDownloadObject_type）
    /// </summary>
    public class PersonalAdditionDownloadHouseLoanItem
    {
        /// <summary>
        /// 贷款类型，长度 1，非必填
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// 首次还款日期，长度 1，非必填
        /// </summary>
        public string FirstRepaymentDate { get; set; } = string.Empty;

        /// <summary>
        /// 贷款期限（月数），长度 1，非必填
        /// </summary>
        public string LoansMonth { get; set; } = string.Empty;

        /// <summary>
        /// 扣除有效期起，长度 20，非必填
        /// </summary>
        public string DeductionBeginDate { get; set; } = string.Empty;

        /// <summary>
        /// 扣除有效期起，长度 20，非必填
        /// </summary>
        public string DeductionEndDate { get; set; } = string.Empty;
    }

    /// <summary>
    /// 住房租金（CsPersonalAdditionDownloadRent）
    /// </summary>
    public class PersonalAdditionDownloadRent
    {
        /// <summary>
        /// 工作城市，长度 6，非必填
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// 房屋坐落地址，长度 8，非必填
        /// </summary>
        public string HouseLocation { get; set; } = string.Empty;

        /// <summary>
        /// 住房租赁合同编号，长度 30，非必填
        /// </summary>
        public string HouseContractNo { get; set; } = string.Empty;

        /// <summary>
        /// 租赁日期起，长度 10，非必填
        /// </summary>
        public string RentBeginDate { get; set; } = string.Empty;

        /// <summary>
        /// 租赁日期止，长度 10，非必填
        /// </summary>
        public string RentEndDate { get; set; } = string.Empty;

        /// <summary>
        /// 扣除有效期起，长度 10，非必填
        /// </summary>
        public string DeductionBeginDate { get; set; } = string.Empty;

        /// <summary>
        /// 扣除有效期止，长度 10，非必填
        /// </summary>
        public string DeductionEndDate { get; set; } = string.Empty;

        /// <summary>
        /// 更新日期，长度 10，非必填
        /// </summary>
        public string UpdateTime { get; set; } = string.Empty;
    }

    /// <summary>
    /// 赡养老人
    /// </summary>
    public class PersonalAdditionDownloadCareOld
    {
        /// <summary>
        /// 是否独生子女，0-否，1-是，长度 1，非必填
        /// </summary>
        public string IsOnlyChild { get; set; } = string.Empty;

        /// <summary>
        /// 分摊方式，长度 1，非必填
        /// </summary>
        public string AllocationMethod { get; set; } = string.Empty;

        /// <summary>
        /// 本年度月扣除金额，长度 6，非必填
        /// </summary>
        public string MonthDeductionAmount { get; set; } = string.Empty;

        /// <summary>
        /// 更新日期，长度 10，非必填
        /// </summary>
        public string UpdateTime { get; set; } = string.Empty;

        /// <summary>
        /// 赡养老人明细，非必填
        /// </summary>
        public List<PersonalAdditionDownloadCareOldItem> CareOldItems { get; set; } = new();
    }

    /// <summary>
    /// 赡养老人明细（CsPersonalAdditionDownloadObjectType）
    /// </summary>
    public class PersonalAdditionDownloadCareOldItem
    {
        /// <summary>
        /// 姓名，长度 8，非必填
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 出生日期，长度 10，非必填
        /// </summary>
        public string Birthday { get; set; } = string.Empty;

        /// <summary>
        /// 身份验证状态，0-待验证，1-验证通过，2-验证不通过，长度 1，非必填
        /// </summary>
        public string VerifyStatus { get; set; } = string.Empty;

        /// <summary>
        /// 扣除有效期止，长度 10，非必填
        /// </summary>
        public string DeductionBeginDate { get; set; } = string.Empty;

        /// <summary>
        /// 扣除有效期起，长度 10，非必填
        /// </summary>
        public string DeductionEndDate { get; set; } = string.Empty;
    }

    /// <summary>
    /// 婴幼儿照护（CsPersonalAdditionDownloadObjectType）
    /// </summary>
    public class PersonalAdditionDownloadCareBaby
    {
        /// <summary>
        /// 姓名，长度 8，非必填
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 出生日期，长度 10，非必填
        /// </summary>
        public string Birthday { get; set; } = string.Empty;

        /// <summary>
        /// 身份验证状态，0-待验证，1-验证通过，2-验证不通过，长度 1，非必填
        /// </summary>
        public string VerifyStatus { get; set; } = string.Empty;

        /// <summary>
        /// 本人扣除比例，长度 3，非必填
        /// </summary>
        public string DeductionPercentage { get; set; } = string.Empty;

        /// <summary>
        /// 扣除有效期起，长度 10，非必填
        /// </summary>
        public string DeductionBeginDate { get; set; } = string.Empty;

        /// <summary>
        /// 扣除有效期止，长度 10，非必填
        /// </summary>
        public string DeductionEndDate { get; set; } = string.Empty;

        /// <summary>
        /// 更新日期，长度 10，非必填
        /// </summary>
        public string UpdateTime { get; set; } = string.Empty;
    }

    /// <summary>
    /// 人员维度错误信息（CsPersonalAdditionDownloadPersonErrorInfo）
    /// </summary>
    public class PersonalAdditionDownloadPersonErrorInfo
    {
        /// <summary>
        /// 证件号码，长度 18，非必填
        /// </summary>
        public string IdCardNo { get; set; } = string.Empty;

        /// <summary>
        /// 证件类型，长度 6，非必填
        /// </summary>
        public string IdCardType { get; set; } = string.Empty;

        /// <summary>
        /// 错误信息，长度 100，非必填
        /// </summary>
        public string ErrorMsg { get; set; } = string.Empty;
    }
}
