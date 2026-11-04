namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 个税收入明细查询响应
    /// </summary>
    public class PersonalTaxIncomeFormQueryResponse
    {
        /// <summary>
        /// 总数量，长度 19，非必填
        /// </summary>
        public long TotalCount { get; set; }

        /// <summary>
        /// 当前页码，长度 19，非必填
        /// </summary>
        public long PageNo { get; set; }

        /// <summary>
        /// 每页条数，长度 19，非必填
        /// </summary>
        public long PageSize { get; set; }

        /// <summary>
        /// 申报企业名称，长度 10，非必填
        /// </summary>
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// 申报税号，长度 18，非必填
        /// </summary>
        public string CustomerTaxNo { get; set; } = string.Empty;

        /// <summary>
        /// 税款所属期，长度 10，非必填
        /// </summary>
        public string DeclarationPeriod { get; set; } = string.Empty;

        /// <summary>
        /// 个税申报表状态，1 未生成，2 生成中，3 已生成，4 生成失败，5 不限，长度 1，非必填
        /// </summary>
        public string PersonalTaxStatus { get; set; } = string.Empty;

        /// <summary>
        /// 申报表明细，非必填
        /// </summary>
        public List<PersonalTaxIncomeFormQueryPersonalTaxForm> PersonalTaxFormList { get; set; } = new();

        /// <summary>
        /// 申报信息，非必填
        /// </summary>
        public List<PersonalTaxIncomeFormQueryInfo> DeclarationInfo { get; set; } = new();

        /// <summary>
        /// 缴款信息，非必填
        /// </summary>
        public List<PersonalTaxIncomeFormQueryPaymentInfo> PaymentInfoList { get; set; } = new();
    }

    /// <summary>
    /// 申报表明细（CsPersonalPersonalTaxIncomeFormQueryPersonalTaxForm）
    /// </summary>
    public class PersonalTaxIncomeFormQueryPersonalTaxForm
    {
        /// <summary>
        /// 序号，长度 1，非必填
        /// </summary>
        public string Seq { get; set; } = string.Empty;

        /// <summary>
        /// 姓名，长度 8，非必填
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 证件类型，201 居民身份证，长度 3，非必填
        /// </summary>
        public string IdCardType { get; set; } = string.Empty;

        /// <summary>
        /// 证件号码，长度 18，非必填
        /// </summary>
        public string IdCardNo { get; set; } = string.Empty;

        /// <summary>
        /// 纳税人识别号，长度 18，非必填
        /// </summary>
        public string TaxIdentifyNumber { get; set; } = string.Empty;

        /// <summary>
        /// 是否为居民个人，0-否，1-是，长度 1，非必填
        /// </summary>
        public string IsIndividualResident { get; set; } = string.Empty;

        /// <summary>
        /// 所得项目代码，长度 4，非必填。正常工资薪金:0101，全年一次性奖金收入:0103，解除劳动合同一次性补偿金:0108，一般劳务报酬所得:0401，保险营销员佣金收入:0402，证券经纪人佣金收入:0403，法律援助补贴劳务报酬:0404，其他连续劳务报酬:0489，稿酬所得:0500，特许权使用费所得:0600
        /// </summary>
        public string ObtainedProjectCode { get; set; } = string.Empty;

        /// <summary>
        /// 所得项目名称，长度 6，非必填。正常工资薪金:0101，全年一次性奖金收入:0103，解除劳动合同一次性补偿金:0108，一般劳务报酬所得:0401，保险营销员佣金收入:0402，证券经纪人佣金收入:0403，法律援助补贴劳务报酬:0404，其他连续劳务报酬:0489，稿酬所得:0500，特许权使用费所得:0600
        /// </summary>
        public string ObtainedProjectName { get; set; } = string.Empty;

        /// <summary>
        /// 本期收入，长度 10，非必填
        /// </summary>
        public string Income { get; set; } = string.Empty;

        /// <summary>
        /// 本期费用，长度 10，非必填
        /// </summary>
        public string Cost { get; set; } = string.Empty;

        /// <summary>
        /// 本期免税收入，长度 10，非必填
        /// </summary>
        public string TaxExemptIncome { get; set; } = string.Empty;

        /// <summary>
        /// 本期减除费用，长度 10，非必填
        /// </summary>
        public string DeductionCost { get; set; } = string.Empty;

        /// <summary>
        /// 本期基本养老保险费，长度 7，非必填
        /// </summary>
        public string PensionInsurancePersonal { get; set; } = string.Empty;

        /// <summary>
        /// 本期基本医疗保险费，长度 6，非必填
        /// </summary>
        public string MedicalInsurancePersonal { get; set; } = string.Empty;

        /// <summary>
        /// 本期失业保险费，长度 5，非必填
        /// </summary>
        public string UnemploymentInsurancePersonal { get; set; } = string.Empty;

        /// <summary>
        /// 本期住房公积金，长度 7，非必填
        /// </summary>
        public string AccumulationFundPersonal { get; set; } = string.Empty;

        /// <summary>
        /// 本期年金，长度 4，非必填
        /// </summary>
        public string EnterpriseAnnuity { get; set; } = string.Empty;

        /// <summary>
        /// 本期商业健康保险，长度 4，非必填
        /// </summary>
        public string CommercialHealthInsurance { get; set; } = string.Empty;

        /// <summary>
        /// 本期税延养老保险，长度 4，非必填
        /// </summary>
        public string TaxDeferredPensionInsurance { get; set; } = string.Empty;

        /// <summary>
        /// 本期财产原值，长度 4，非必填
        /// </summary>
        public string PropertyOriginValue { get; set; } = string.Empty;

        /// <summary>
        /// 本期允许扣除的税费，长度 4，非必填
        /// </summary>
        public string AllowableTaxDeductions { get; set; } = string.Empty;

        /// <summary>
        /// 本期其他，长度 4，非必填
        /// </summary>
        public string Other { get; set; } = string.Empty;

        /// <summary>
        /// 累计收入，长度 8，非必填
        /// </summary>
        public string AccumulatedIncomeAmount { get; set; } = string.Empty;

        /// <summary>
        /// 累计费用，长度 4，非必填
        /// </summary>
        public string AccumulatedCost { get; set; } = string.Empty;

        /// <summary>
        /// 累计收入额，长度 8，非必填
        /// </summary>
        public string AccumulatedIncome { get; set; } = string.Empty;

        /// <summary>
        /// 累计减除费用，长度 8，非必填
        /// </summary>
        public string AccumulatedDeductionExpenses { get; set; } = string.Empty;

        /// <summary>
        /// 累计专项扣除，长度 7，非必填
        /// </summary>
        public string AccumulatedSpecialDeductions { get; set; } = string.Empty;

        /// <summary>
        /// 累计子女教育，长度 7，非必填
        /// </summary>
        public string AccumulatedExpenditureChildrenEducation { get; set; } = string.Empty;

        /// <summary>
        /// 累计赡养老人，长度 7，非必填
        /// </summary>
        public string AccumulatedCaringElderly { get; set; } = string.Empty;

        /// <summary>
        /// 累计住房贷款利息，长度 7，非必填
        /// </summary>
        public string AccumulatedHousingLoanInterest { get; set; } = string.Empty;

        /// <summary>
        /// 累计住房租金，长度 4，非必填
        /// </summary>
        public string AccumulatedHousingRent { get; set; } = string.Empty;

        /// <summary>
        /// 累计继续教育，长度 4，非必填
        /// </summary>
        public string AccumulatedContinuingEducation { get; set; } = string.Empty;

        /// <summary>
        /// 累计 3 岁以下婴幼儿照护，长度 4，非必填
        /// </summary>
        public string AccumulatedCareChildrenUnderThreeYearsOld { get; set; } = string.Empty;

        /// <summary>
        /// 累计个人养老金，长度 4，非必填
        /// </summary>
        public string AccumulatedPersonalPension { get; set; } = string.Empty;

        /// <summary>
        /// 累计其他扣除，长度 4，非必填
        /// </summary>
        public string AccumulatedOtherDeductions { get; set; } = string.Empty;

        /// <summary>
        /// 减按计税比例，长度 3，非必填
        /// </summary>
        public string TaxCalculationRationReduce { get; set; } = string.Empty;

        /// <summary>
        /// 准予扣除的捐赠额，长度 4，非必填
        /// </summary>
        public string DonationAmountAllowedForDeduction { get; set; } = string.Empty;

        /// <summary>
        /// 应纳税所得额，长度 8，非必填
        /// </summary>
        public string AccumulatedTaxableIncome { get; set; } = string.Empty;

        /// <summary>
        /// 税率/预扣率，长度 1，非必填
        /// </summary>
        public string TaxRate { get; set; } = string.Empty;

        /// <summary>
        /// 速算扣除数，长度 4，非必填
        /// </summary>
        public string QuickCalculationDeduction { get; set; } = string.Empty;

        /// <summary>
        /// 应纳税额，长度 6，非必填
        /// </summary>
        public string AccumulatedTaxableAmount { get; set; } = string.Empty;

        /// <summary>
        /// 减免税额，长度 4，非必填
        /// </summary>
        public string TaxSavings { get; set; } = string.Empty;

        /// <summary>
        /// 累计减免税额，长度 10，非必填
        /// </summary>
        public string AccumulatedTaxReductionsExemptions { get; set; } = string.Empty;

        /// <summary>
        /// 已缴税额，长度 4，非必填
        /// </summary>
        public string AccumulatedPaidTaxAmount { get; set; } = string.Empty;

        /// <summary>
        /// 应补/退税额，长度 6，非必填
        /// </summary>
        public string TaxDeclaration { get; set; } = string.Empty;

        /// <summary>
        /// 备注，长度 1，非必填
        /// </summary>
        public string Remark { get; set; } = string.Empty;
    }

    /// <summary>
    /// 申报信息（CsPersonalPersonalTaxIncomeFormQueryInfo）
    /// </summary>
    public class PersonalTaxIncomeFormQueryInfo
    {
        /// <summary>
        /// 申报人次，长度 2，非必填
        /// </summary>
        public string DeclarationNumber { get; set; } = string.Empty;

        /// <summary>
        /// 本期收入，长度 10，非必填
        /// </summary>
        public string TotalIncome { get; set; } = string.Empty;

        /// <summary>
        /// 应补（退）税额，长度 10，非必填
        /// </summary>
        public string TotalTaxAmount { get; set; } = string.Empty;

        /// <summary>
        /// 申报类型，0：正常申报，1：更正申报，长度 1，非必填
        /// </summary>
        public string DeclarationType { get; set; } = string.Empty;

        /// <summary>
        /// 申报日期，长度 10，非必填
        /// </summary>
        public string DeclareTime { get; set; } = string.Empty;

        /// <summary>
        /// 申报流水号，长度 20，非必填
        /// </summary>
        public string DeclareSerialNo { get; set; } = string.Empty;

        /// <summary>
        /// 个税收入报表类型，0-综合所得，1-分类所得，长度 1，非必填
        /// </summary>
        public string PersonalTaxFormType { get; set; } = string.Empty;

        /// <summary>
        /// 申报状态，1-未申报，2-申报中，3-已申报，4-申报失败，长度 1，非必填
        /// </summary>
        public string DeclareState { get; set; } = string.Empty;
    }

    /// <summary>
    /// 缴款信息（CsPersonalPersonalTaxIncomeFormQueryPaymentInfo）
    /// </summary>
    public class PersonalTaxIncomeFormQueryPaymentInfo
    {
        /// <summary>
        /// 缴税金额，长度 7，非必填
        /// </summary>
        public string DueTaxAmount { get; set; } = string.Empty;

        /// <summary>
        /// 滞纳金，长度 4，非必填
        /// </summary>
        public string LateFee { get; set; } = string.Empty;

        /// <summary>
        /// 缴税期限，长度 10，非必填
        /// </summary>
        public string PaymentDeadline { get; set; } = string.Empty;

        /// <summary>
        /// 缴款状态，0-未缴款，1-已缴款，2-无需缴款，3-无缴款状态，4-缴款中，5-缴款失败，长度 1，非必填
        /// </summary>
        public string PaymentStatus { get; set; } = string.Empty;

        /// <summary>
        /// 缴款日期，长度 10，非必填
        /// </summary>
        public string PaymentDate { get; set; } = string.Empty;

        /// <summary>
        /// 电子税票号，长度 18，非必填
        /// </summary>
        public string DigitTaxReceiptNo { get; set; } = string.Empty;

        /// <summary>
        /// 缴款方式：税库联网缴税，长度 6，非必填
        /// </summary>
        public string PaymentType { get; set; } = string.Empty;
    }
}
