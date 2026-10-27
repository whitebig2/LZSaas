using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.BusinessModels
{
    /// <summary>
    /// 账户信息实体类
    /// </summary>
    public class T0005ResultSetItem : ICzyResultSetItem
    {
        /// <summary>
        /// 直联编号
        /// </summary>
        public string DirectCode { get; set; }

        /// <summary>
        /// 账户唯一标识
        /// </summary>
        public string AcctKey { get; set; }

        /// <summary>
        /// 账户号
        /// </summary>
        public string AcctNo { get; set; }

        /// <summary>
        /// 账户名称
        /// </summary>
        public string AcctName { get; set; }

        /// <summary>
        /// 账户开户日期
        /// </summary>
        public string AcctOpenDate { get; set; }

        /// <summary>
        /// 币种编码
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// 币种名称
        /// </summary>
        public string CurrencyName { get; set; }

        /// <summary>
        /// 直连属性(1,直连 2,非直连 3,swift)
        /// </summary>
        public string AcctInWay { get; set; }

        /// <summary>
        /// 账户性质(1,基本存款账户 2,一般存款账户 3,专用存款账户 4,临时存款账户)
        /// </summary>
        public string AcctNature { get; set; }

        /// <summary>
        /// 收付属性：1 收入户、2 支出户、3 收支户
        /// </summary>
        public string PayReceiveAttr { get; set; }

        /// <summary>
        /// 是否开通网银 1,是 2，否
        /// </summary>
        public string AcctEbankType { get; set; }

        /// <summary>
        /// 关联账户科目编号
        /// </summary>
        public string SubjectCode { get; set; }

        /// <summary>
        /// 是否加入资金池 1,是 2，否
        /// </summary>
        public string CashPoolFlag { get; set; }

        /// <summary>
        /// 1：冻结，2：解冻
        /// </summary>
        public string FreezeStatus { get; set; }

        /// <summary>
        /// 账户状态：1.正常 2.变更中 3.销户中 4.已销户
        /// </summary>
        public string AcctStatus { get; set; }

        /// <summary>
        /// 前置机编号
        /// </summary>
        public string FrontPcCode { get; set; }

        /// <summary>
        /// (所属)组织id
        /// </summary>
        public string OrgKey { get; set; }

        /// <summary>
        /// (所属)组织编号
        /// </summary>
        public string OrgCode { get; set; }

        /// <summary>
        /// (所属)组织名称
        /// </summary>
        public string OrgName { get; set; }

        /// <summary>
        /// 使用单位ID
        /// </summary>
        public string UseOrgKey { get; set; }

        /// <summary>
        /// 使用单位编号
        /// </summary>
        public string UseOrgCode { get; set; }

        /// <summary>
        /// 使用单位名称
        /// </summary>
        public string UseOrgName { get; set; }

        /// <summary>
        /// 授权单位ID
        /// </summary>
        public string AuthorizedOrgKey { get; set; }

        /// <summary>
        /// 授权单位编号
        /// </summary>
        public string AuthorizedOrgCode { get; set; }

        /// <summary>
        /// 授权单位名称
        /// </summary>
        public string AuthorizedOrgName { get; set; }

        /// <summary>
        /// 法人单位ID
        /// </summary>
        public string LegalPersonOrgKey { get; set; }

        /// <summary>
        /// 法人单位编号
        /// </summary>
        public string LegalPersonOrgCode { get; set; }

        /// <summary>
        /// 法人单位名称
        /// </summary>
        public string LegalPersonOrgName { get; set; }

        /// <summary>
        /// 开户行所在省编码
        /// </summary>
        public string AcctProvinceCode { get; set; }

        /// <summary>
        /// 开户行所在省名称
        /// </summary>
        public string AcctProvinceName { get; set; }

        /// <summary>
        /// 开户行所在城市编码
        /// </summary>
        public string AcctCityCode { get; set; }

        /// <summary>
        /// 开户行所在城市名称
        /// </summary>
        public string AcctCityName { get; set; }

        /// <summary>
        /// 合作金融机构ID,当BusiAcctTypeCode为内部账户时，该编码为单位ID
        /// </summary>
        public string CooperativeInstitutionId { get; set; }

        /// <summary>
        /// 合作金融机构编码,当BusiAcctTypeCode为银行账户时，该编码为银行大类编码；当BusiAcctTypeCode为内部账户时，该编码为单位编码
        /// </summary>
        public string CooperativeInstitutionCode { get; set; }

        /// <summary>
        /// 合作金融机构名称,当BusiAcctTypeCode为银行账户时，该编码为银行大类名称当BusiAcctTypeCode为内部账户时，该编码为单位名称
        /// </summary>
        public string CooperativeInstitutionName { get; set; }

        /// <summary>
        /// 合作金融机构网点编号,当BusiAcctTypeCode为银行账户时，该编码为开户支行编码当BusiAcctTypeCode为内部账户时，该编码为单位编码
        /// </summary>
        public string CooperativeFinancNetCode { get; set; }

        /// <summary>
        /// 合作金融机构网点名称,当BusiAcctTypeCode为银行账户时，该编码为开户支行名称当BusiAcctTypeCode为内部账户时，该编码为单位名称
        /// </summary>
        public string CooperativeFinancNetName { get; set; }

        /// <summary>
        /// 业务模块使用的账户类型编号,银行账户/内部账户，参考常量：com.iss.cms.common.constant.GlobalConstant.BusiAcctType
        /// </summary>
        public string BusiAcctTypeCode { get; set; }

        /// <summary>
        /// 业务模块使用的账户类型名称,银行账户/内部账户，参考常量：com.iss.cms.common.constant.GlobalConstant.BusiAcctType
        /// </summary>
        public string BusiAcctTypeName { get; set; }

        /// <summary>
        /// 唯一key
        /// </summary>
        public string OnlyKey { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public string Balance { get; set; }

        /// <summary>
        /// 可用余额
        /// </summary>
        public string UsableBalance { get; set; }

        /// <summary>
        /// 冻结金额
        /// </summary>
        public string FreezeAmount { get; set; }

        /// <summary>
        /// 合作金融机构类型 1.银行 2.第三方 3.结算中心 4.财务公司
        /// </summary>
        public string CooperativeInstitutionType { get; set; }
    }



}




