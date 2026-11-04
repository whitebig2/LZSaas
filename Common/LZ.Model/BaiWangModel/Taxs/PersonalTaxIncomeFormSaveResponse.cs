namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 个税收入明细保存响应
    /// </summary>
    public class PersonalTaxIncomeFormSaveResponse
    {
        /// <summary>
        /// 保存成功列表，非必填
        /// </summary>
        public List<PersonalTaxIncomeFormSaveSuccess> SuccessList { get; set; } = new();

        /// <summary>
        /// 保存失败列表，非必填
        /// </summary>
        public List<PersonalTaxIncomeFormSaveFail> FailList { get; set; } = new();
    }

    /// <summary>
    /// 个税收入明细保存成功项（CsPersonalPersonalTaxIncomeFormSaveSuccess）
    /// </summary>
    public class PersonalTaxIncomeFormSaveSuccess
    {
        /// <summary>
        /// 收入 uuid，长度 36，非必填
        /// </summary>
        public string Uuid { get; set; } = string.Empty;

        /// <summary>
        /// 序号，长度 1，非必填
        /// </summary>
        public string Seq { get; set; } = string.Empty;

        /// <summary>
        /// 姓名，长度 2，非必填
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 证件类型，长度 10，非必填
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
        /// 所得项目代码，长度 4，非必填
        /// </summary>
        public string ObtainedProjectCode { get; set; } = string.Empty;

        /// <summary>
        /// 所得项目名称，长度 6，非必填
        /// </summary>
        public string ObtainedProjectName { get; set; } = string.Empty;

        /// <summary>
        /// 本期收入，长度 8，非必填
        /// </summary>
        public string Income { get; set; } = string.Empty;

        /// <summary>
        /// 本期费用，长度 4，非必填
        /// </summary>
        public string Cost { get; set; } = string.Empty;

        /// <summary>
        /// 本期免税收入，长度 4，非必填
        /// </summary>
        public string TaxExemptIncome { get; set; } = string.Empty;

        /// <summary>
        /// 本期减除费用，长度 7，非必填
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
        /// 已缴税额，长度 4，非必填
        /// </summary>
        public string AccumulatedPaidTaxAmount { get; set; } = string.Empty;

        /// <summary>
        /// 应补/退税额，长度 6，非必填
        /// </summary>
        public string TaxDeclaration { get; set; } = string.Empty;

        /// <summary>
        /// 备注，长度 4，非必填
        /// </summary>
        public string Remark { get; set; } = string.Empty;
    }

    /// <summary>
    /// 个税收入明细保存失败项（CsPersonalPersonalTaxIncomeFormSaveFail）
    /// </summary>
    public class PersonalTaxIncomeFormSaveFail
    {
        /// <summary>
        /// 序号，长度 1，非必填
        /// </summary>
        public string Seq { get; set; } = string.Empty;

        /// <summary>
        /// 姓名，长度 2，非必填
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 证件类型，长度 3，非必填
        /// </summary>
        public string IdCardType { get; set; } = string.Empty;

        /// <summary>
        /// 证件号码，长度 18，非必填
        /// </summary>
        public string IdCardNo { get; set; } = string.Empty;

        /// <summary>
        /// 错误原因，长度 24，非必填
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
