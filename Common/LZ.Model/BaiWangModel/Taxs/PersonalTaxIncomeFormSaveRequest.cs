namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 个税收入明细保存请求
    /// </summary>
    public class PersonalTaxIncomeFormSaveRequest
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
        /// 税款所属期，格式 yyyy-MM，长度 10，必填
        /// </summary>
        public required string DeclarationPeriod { get; set; } = string.Empty;

        /// <summary>
        /// 操作类型，长度 10，必填
        /// </summary>
        public required string OperateType { get; set; } = string.Empty;

        /// <summary>
        /// 个税人员收入明细，必填
        /// </summary>
        public required List<PersonalTaxIncomeFormSaveObject> PersonalTaxFormList { get; set; } = new();
    }

    /// <summary>
    /// 个税人员收入明细（CsPersonalPersonalTaxIncomeFormSaveObjectType）
    /// </summary>
    public class PersonalTaxIncomeFormSaveObject
    {
        /// <summary>
        /// 姓名，长度 50，必填
        /// </summary>
        public required string Name { get; set; } = string.Empty;

        /// <summary>
        /// 证件类型，201 居民身份证，长度 6，必填
        /// </summary>
        public required string IdCardType { get; set; } = string.Empty;

        /// <summary>
        /// 证件号码，证件类型为居民身份证时必须为 18 位，长度 18，必填
        /// </summary>
        public required string IdCardNo { get; set; } = string.Empty;

        /// <summary>
        /// 所得项目代码，长度 10，必填。正常工资薪金:0101，全年一次性奖金收入:0103，解除劳动合同一次性补偿金:0108，一般劳务报酬所得:0401，保险营销员佣金收入:0402，证券经纪人佣金收入:0403，法律援助补贴劳务报酬:0404，其他连续劳务报酬:0489，稿酬所得:0500，特许权使用费所得:0600
        /// </summary>
        public required string ObtainedProjectCode { get; set; } = string.Empty;

        /// <summary>
        /// 所得项目名称，长度 50，必填。正常工资薪金:0101，全年一次性奖金收入:0103，解除劳动合同一次性补偿金:0108，一般劳务报酬所得:0401，保险营销员佣金收入:0402，证券经纪人佣金收入:0403，法律援助补贴劳务报酬:0404，其他连续劳务报酬:0489，稿酬所得:0500，特许权使用费所得:0600
        /// </summary>
        public required string ObtainedProjectName { get; set; } = string.Empty;

        /// <summary>
        /// 本期收入，长度 12，必填
        /// </summary>
        public required string Income { get; set; } = string.Empty;

        /// <summary>
        /// 本期免税收入，长度 12，非必填
        /// </summary>
        public string? TaxExemptIncome { get; set; }

        /// <summary>
        /// 基本养老保险费，长度 12，非必填
        /// </summary>
        public string? PensionInsurancePersonal { get; set; }

        /// <summary>
        /// 基本医疗保险费，长度 12，非必填
        /// </summary>
        public string? MedicalInsurancePersonal { get; set; }

        /// <summary>
        /// 失业保险费，长度 12，非必填
        /// </summary>
        public string? UnemploymentInsurancePersonal { get; set; }

        /// <summary>
        /// 住房公积金，长度 12，非必填
        /// </summary>
        public string? AccumulationFundPersonal { get; set; }

        /// <summary>
        /// 累计子女教育，长度 12，非必填
        /// </summary>
        public string? AccumulatedExpenditureChildrenEducation { get; set; }

        /// <summary>
        /// 累计继续教育，长度 12，非必填
        /// </summary>
        public string? AccumulatedContinuingEducation { get; set; }

        /// <summary>
        /// 累计住房贷款利息，长度 12，非必填
        /// </summary>
        public string? AccumulatedHousingLoanInterest { get; set; }

        /// <summary>
        /// 累计住房租金，长度 12，非必填
        /// </summary>
        public string? AccumulatedHousingRent { get; set; }

        /// <summary>
        /// 累计赡养老人，长度 12，非必填
        /// </summary>
        public string? AccumulatedCaringElderly { get; set; }

        /// <summary>
        /// 累计 3 岁以下婴幼儿照护，长度 12，非必填
        /// </summary>
        public string? AccumulatedCareChildrenUnderThreeYearsOld { get; set; }

        /// <summary>
        /// 累计个人养老金，长度 12，非必填
        /// </summary>
        public string? AccumulatedPersonalPension { get; set; }

        /// <summary>
        /// 企业（职业）年金，长度 12，非必填
        /// </summary>
        public string? EnterpriseAnnuity { get; set; }

        /// <summary>
        /// 商业健康保险，长度 12，非必填
        /// </summary>
        public string? CommercialHealthInsurance { get; set; }

        /// <summary>
        /// 税延养老保险，长度 12，非必填
        /// </summary>
        public string? TaxDeferredPensionInsurance { get; set; }

        /// <summary>
        /// 其他，长度 12，非必填
        /// </summary>
        public string? Other { get; set; }

        /// <summary>
        /// 准予扣除的捐赠额，长度 12，非必填
        /// </summary>
        public string? DonationAmountAllowedForDeduction { get; set; }

        /// <summary>
        /// 减免税额，长度 12，非必填
        /// </summary>
        public string? TaxSavings { get; set; }

        /// <summary>
        /// 备注，长度 500，非必填
        /// </summary>
        public string? Remark { get; set; }

        /// <summary>
        /// 允许扣除的税费，长度 12，非必填
        /// </summary>
        public string? AllowableTaxDeductions { get; set; }

        /// <summary>
        /// 收入 uuid，同步类型为更新时必填，传新增成功返回的收入 uuid，长度 64，条件必填
        /// </summary>
        public string? Uuid { get; set; }

        /// <summary>
        /// 序号，从 1 递增，长度 32，非必填
        /// </summary>
        public string? Seq { get; set; }
    }
}
