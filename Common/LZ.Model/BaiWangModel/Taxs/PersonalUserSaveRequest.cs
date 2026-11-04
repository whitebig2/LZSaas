namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 人员信息保存请求
    /// </summary>
    public class PersonalUserSaveRequest
    {
        /// <summary>
        /// 代理机构税号，长度 50，必填
        /// </summary>
        public required string BelongTaxNo { get; set; } = string.Empty;

        /// <summary>
        /// 客户税号，长度 50，必填
        /// </summary>
        public required string CustomerTaxNo { get; set; } = string.Empty;

        /// <summary>
        /// 是否报送，0-否，1-是，长度 1，非必填
        /// </summary>
        public string? IsDeclare { get; set; }

        /// <summary>
        /// 来源，长度 20，非必填
        /// </summary>
        public string? DataSource { get; set; }

        /// <summary>
        /// 人员信息，必填
        /// </summary>
        public required List<PersonalUserSaveObject> UserList { get; set; } = new();
    }

    /// <summary>
    /// 人员信息（CsPersonalUserSaveObjectType）
    /// </summary>
    public class PersonalUserSaveObject
    {
        /// <summary>
        /// 工号，长度 50，非必填
        /// </summary>
        public string? JobNo { get; set; }

        /// <summary>
        /// 姓名，长度 50，必填
        /// </summary>
        public required string Name { get; set; } = string.Empty;

        /// <summary>
        /// 证件类型，201-居民身份证，长度 3，必填
        /// </summary>
        public required string IdCardType { get; set; } = string.Empty;

        /// <summary>
        /// 证件号码，证件类型为居民身份证时必须为 18 位，长度 18，必填
        /// </summary>
        public required string IdCardNo { get; set; } = string.Empty;

        /// <summary>
        /// 国籍。证件类型为居民身份证时允许为空，为空时默认为中国。枚举值参考 https://docs.qq.com/sheet/DV0ptVUdDWmNjRkxk?tab=000004 ，长度 3，非必填
        /// </summary>
        public string? Nationality { get; set; }

        /// <summary>
        /// 性别，1-男，2-女。证件类型为居民身份证时允许为空，为空时系统根据证件号码解析。长度 1，非必填
        /// </summary>
        public string? Sex { get; set; }

        /// <summary>
        /// 出生日期，格式 yyyy-MM-dd，如 1988-11-01。证件类型为居民身份证时允许为空，为空时系统根据证件号码解析。长度 10，非必填
        /// </summary>
        public string? Birthday { get; set; }

        /// <summary>
        /// 人员状态。文档同时给出 0-正常、1-非正常，以及 1-正常、2-非正常。允许为空。为空时离职日期为空则人员状态为正常，离职日期有值则人员状态为非正常。长度 1，非必填
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// 受雇从业类型，10-雇员，21-保险营销员，22-证券经纪人，23-实习学生（全日制学历教育），29-其他，长度 2，必填
        /// </summary>
        public required string EmploymentType { get; set; } = string.Empty;

        /// <summary>
        /// 其他情况说明，任职受雇从业类型为其他时必填，0-扣缴申报利息股息红利所得，1-扣缴申报偶然所得，长度 255，条件必填
        /// </summary>
        public string? OtherExplain { get; set; }

        /// <summary>
        /// 入职年度就业情形，1-当年首次入职学生，99-当年首次入职其他人员。任职受雇从业类型为雇员、保险营销员、证券经纪人时可传，否则默认为空。长度 2，非必填
        /// </summary>
        public string? EmploymentAnnualSituation { get; set; }

        /// <summary>
        /// 手机号码，长度 11，非必填
        /// </summary>
        public string? Mobile { get; set; }

        /// <summary>
        /// 受雇从业日期，格式 yyyy-MM-dd，长度 10，必填
        /// </summary>
        public required string EmploymentDate { get; set; } = string.Empty;

        /// <summary>
        /// 离职日期，格式 yyyy-MM-dd，长度 15，非必填
        /// </summary>
        public string? ResignationDate { get; set; }

        /// <summary>
        /// 是否残疾，0-否，1-是，长度 1，必填
        /// </summary>
        public required string IsDisability { get; set; } = string.Empty;

        /// <summary>
        /// 残疾证件类型，残疾时必填，11-残疾证，12-残疾军人证，13-伤残人民警察证，14-残疾消防救援人员证，15-伤残预备役人员、伤残民兵民工证，16-因公伤残人员证，长度 2，条件必填
        /// </summary>
        public string? DisabilityCardType { get; set; }

        /// <summary>
        /// 残疾证件号码，残疾时必填，长度 50，条件必填
        /// </summary>
        public string? DisabilityCardNo { get; set; }

        /// <summary>
        /// 是否烈属，0-否，1-是，长度 1，必填
        /// </summary>
        public required string IsMartyrFamilies { get; set; } = string.Empty;

        /// <summary>
        /// 烈属证号，烈属时必填，长度 50，条件必填
        /// </summary>
        public string? MartyrFamiliesCardNo { get; set; }

        /// <summary>
        /// 是否孤老，0-否，1-是，长度 1，必填
        /// </summary>
        public required string IsLonelyOld { get; set; } = string.Empty;

        /// <summary>
        /// 是否扣除减除费用，0-否，1-是，长度 1，非必填
        /// </summary>
        public string? IsDeductExpense { get; set; }

        /// <summary>
        /// 学历，10-研究生，20-大学本科，30-大学本科以下，长度 2，非必填
        /// </summary>
        public string? Education { get; set; }

        /// <summary>
        /// 职务，1-高层，9-普通，长度 2，非必填
        /// </summary>
        public string? Duty { get; set; }

        /// <summary>
        /// 电子邮箱，长度 50，非必填
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// 备注，长度 255，非必填
        /// </summary>
        public string? Remark { get; set; }

        /// <summary>
        /// 户籍所在地（省）编码，枚举值参考地区编码附表，长度 10，非必填
        /// </summary>
        public string? RegisteredResidenceProvinceCode { get; set; }

        /// <summary>
        /// 户籍所在地（省），枚举值参考地区编码附表，长度 20，非必填
        /// </summary>
        public string? RegisteredResidenceProvince { get; set; }

        /// <summary>
        /// 户籍所在地（市）编码，枚举值参考地区编码附表，长度 10，非必填
        /// </summary>
        public string? RegisteredResidenceCityCode { get; set; }

        /// <summary>
        /// 户籍所在地（市），枚举值参考地区编码附表，长度 20，非必填
        /// </summary>
        public string? RegisteredResidenceCity { get; set; }

        /// <summary>
        /// 户籍所在地（区）编码，枚举值参考地区编码附表，长度 10，非必填
        /// </summary>
        public string? RegisteredResidenceAreaCode { get; set; }

        /// <summary>
        /// 户籍所在地（区），枚举值参考地区编码附表，长度 20，非必填
        /// </summary>
        public string? RegisteredResidenceArea { get; set; }

        /// <summary>
        /// 户籍所在地（街道）编码，枚举值参考地区编码附表，长度 10，非必填
        /// </summary>
        public string? RegisteredResidenceStreetCode { get; set; }

        /// <summary>
        /// 户籍所在地（街道），枚举值参考地区编码附表，长度 20，非必填
        /// </summary>
        public string? RegisteredResidenceStreet { get; set; }

        /// <summary>
        /// 户籍所在地详细地址，长度 50，非必填
        /// </summary>
        public string? RegisteredResidenceDetail { get; set; }

        /// <summary>
        /// 经常居住地（省）编码，枚举值参考地区编码附表，长度 10，非必填
        /// </summary>
        public string? FrequentResidenceProvinceCode { get; set; }

        /// <summary>
        /// 经常居住地（省），枚举值参考地区编码附表，长度 20，非必填
        /// </summary>
        public string? FrequentResidenceProvince { get; set; }

        /// <summary>
        /// 经常居住地（市）编码，枚举值参考地区编码附表，长度 10，非必填
        /// </summary>
        public string? FrequentResidenceCityCode { get; set; }

        /// <summary>
        /// 经常居住地（市），枚举值参考地区编码附表，长度 20，非必填
        /// </summary>
        public string? FrequentResidenceCity { get; set; }

        /// <summary>
        /// 经常居住地（区）编码，枚举值参考地区编码附表，长度 10，非必填
        /// </summary>
        public string? FrequentResidenceAreaCode { get; set; }

        /// <summary>
        /// 经常居住地（区），枚举值参考地区编码附表，长度 20，非必填
        /// </summary>
        public string? FrequentResidenceArea { get; set; }

        /// <summary>
        /// 经常居住地（街道）编码，枚举值参考地区编码附表，长度 10，非必填
        /// </summary>
        public string? FrequentResidenceStreetCode { get; set; }

        /// <summary>
        /// 经常居住地（街道），枚举值参考地区编码附表，长度 20，非必填
        /// </summary>
        public string? FrequentResidenceStreet { get; set; }

        /// <summary>
        /// 经常居住地详细地址，长度 50，非必填
        /// </summary>
        public string? FrequentResidenceDetail { get; set; }

        /// <summary>
        /// 联系地址（省）编码，枚举值参考地区编码附表，长度 10，非必填
        /// </summary>
        public string? ContactAddressProvinceCode { get; set; }

        /// <summary>
        /// 联系地址（省），枚举值参考地区编码附表，长度 20，非必填
        /// </summary>
        public string? ContactAddressProvince { get; set; }

        /// <summary>
        /// 联系地址（市）编码，枚举值参考地区编码附表，长度 10，非必填
        /// </summary>
        public string? ContactAddressCityCode { get; set; }

        /// <summary>
        /// 联系地址（市），枚举值参考地区编码附表，长度 20，非必填
        /// </summary>
        public string? ContactAddressCity { get; set; }

        /// <summary>
        /// 联系地址（区）编码，枚举值参考地区编码附表，长度 10，非必填
        /// </summary>
        public string? ContactAddressAreaCode { get; set; }

        /// <summary>
        /// 联系地址（区），枚举值参考地区编码附表，长度 20，非必填
        /// </summary>
        public string? ContactAddressArea { get; set; }

        /// <summary>
        /// 联系地址（街道）编码，枚举值参考地区编码附表，长度 10，非必填
        /// </summary>
        public string? ContactAddressStreetCode { get; set; }

        /// <summary>
        /// 联系地址（街道），长度 20，非必填
        /// </summary>
        public string? ContactAddressStreet { get; set; }

        /// <summary>
        /// 联系地址详细地址，长度 50，非必填
        /// </summary>
        public string? ContactAddressDetail { get; set; }

        /// <summary>
        /// 开户银行编码。开户银行编码、银行账号和开户行省份同时为空或同时有值。枚举值参考附表，长度 10，非必填
        /// </summary>
        public string? SellerBankCode { get; set; }

        /// <summary>
        /// 银行账号，长度 50，非必填
        /// </summary>
        public string? SellerBankNumber { get; set; }

        /// <summary>
        /// 开户行省份编码，枚举值参考地区编码附表，长度 10，非必填
        /// </summary>
        public string? SellerBankProvinceCode { get; set; }

        /// <summary>
        /// 个人投资额，精确到小数点后 2 位。个人投资额和个人投资比例必须同时为空或者同时大于 0。长度 12，非必填
        /// </summary>
        public string? PersonalInvestmentAmount { get; set; }

        /// <summary>
        /// 个人投资比例（%），精确到小数点后 6 位。个人投资额大于 0 时必填，且范围为 (0,100]，否则为空。长度 3，条件必填
        /// </summary>
        public string? PersonalInvestmentPercentage { get; set; }
    }
}
