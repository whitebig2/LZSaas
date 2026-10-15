using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.ParamSetModels
{

    /// <summary>
    /// 1.2.5. T0005-账户信息查询接口
    /// </summary>
    public class T0005ParamSetItem : ICzyParamSetItem
    {
        /// <summary>
        /// 所属单位ID数组
        /// </summary>
        public List<string> OrgKeyList { get; set; }

        /// <summary>
        /// 使用单位ID数组
        /// </summary>
        public List<string> UseOrgKeyList { get; set; }

        /// <summary>
        /// 授权单位ID数组
        /// </summary>
        public List<string> AuthorizedOrgKeyList { get; set; }

        /// <summary>
        /// 法人单位ID数组
        /// </summary>
        public List<string> LegalPersonOrgKeyList { get; set; }

        /// <summary>
        /// 账户ID数组
        /// </summary>
        public string AcctKeyList { get; set; }

        /// <summary>
        /// 币种编号数组
        /// </summary>
        public string CurrencyCodeList { get; set; }

        /// <summary>
        /// 账户类别
        /// </summary>
        public string CategoryCode { get; set; }

        /// <summary>
        /// 账户类别
        /// </summary>
        public string CategoryCodeList { get; set; }

        /// <summary>
        /// 账户编号数组
        /// </summary>
        public string AcctNoList { get; set; }

        /// <summary>
        /// 账户号
        /// </summary>
        public string AcctNo { get; set; }

        /// <summary>
        /// 账户名称
        /// </summary>
        public string AcctName { get; set; }

        /// <summary>
        /// 账户名或账户名称（根据该字段模糊匹配账户编号或账户名）
        /// </summary>
        public string AcctNoOrName { get; set; }

        /// <summary>
        /// 账户号（模糊匹配）
        /// </summary>
        public string AcctNoLike { get; set; }

        /// <summary>
        /// 账户名称（模糊匹配）
        /// </summary>
        public string AcctNameLike { get; set; }

        /// <summary>
        /// 账户模块使用的账户类型编号列表
        /// </summary>
        public string CooperativeInstitutionTypeList { get; set; }

        /// <summary>
        /// 业务模块使用的账户类型
        /// </summary>
        public string BusiAcctType { get; set; }

        /// <summary>
        /// 1：冻结，2：解冻
        /// </summary>
        public string InProcessStatus { get; set; }

        /// <summary>
        /// 合作金融机构编码
        /// </summary>
        public string CooperativeInstitutionCode { get; set; }

        /// <summary>
        /// 合作金融机构名称
        /// </summary>
        public string CooperativeInstitutionName { get; set; }

        /// <summary>
        /// 直联编号
        /// </summary>
        public string DirectCode { get; set; }

        /// <summary>
        /// 合作金融机构网点编号
        /// </summary>
        public string CooperativeFinancNetCode { get; set; }

        /// <summary>
        /// 合作金融机构网点名称
        /// </summary>
        public string CooperativeFinancNetName { get; set; }

        /// <summary>
        /// 是否股份虚拟户
        /// </summary>
        public string StockVirtualAcctStatus { get; set; }

        /// <summary>
        /// 股份虚拟户类型
        /// </summary>
        public string StockVirtualAcctType { get; set; }

        /// <summary>
        /// 是否包含冻结状态的账户
        /// </summary>
        public string FreezeStatus { get; set; }

        /// <summary>
        /// 冻结类型
        /// </summary>
        public string FreezeTypeList { get; set; }

        /// <summary>
        /// 直连属性(1,直连;2,非直连 3,swift)
        /// </summary>
        public string AcctInWay { get; set; }

        /// <summary>
        /// 账户性质(1,基本存款账户;2,一般存款账户;3,专用存款账户;4,临时存款账户)
        /// </summary>
        public string AcctNature { get; set; }

        /// <summary>
        /// 账户收支属性
        /// </summary>
        public string PayReceiveAttrList { get; set; }

        /// <summary>
        /// 币种编号
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// 所属公司ID
        /// </summary>
        public string OrgKey { get; set; }

        /// <summary>
        /// 使用单位ID
        /// </summary>
        public string UseOrgKey { get; set; }

        /// <summary>
        /// 授权单位ID
        /// </summary>
        public string AuthorizedOrgKey { get; set; }

        /// <summary>
        /// 法人单位ID
        /// </summary>
        public string LegalPersonOrgKey { get; set; }

        /// <summary>
        /// 模块编号
        /// </summary>
        public string ModuleCode { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 是否去重
        /// </summary>
        public string DistinctStatus { get; set; }

        /// <summary>
        /// 是否包含所有状态的账户（比如销户状态）
        /// </summary>
        public string IncludeAllAcctStatus { get; set; }

        /// <summary>
        /// 使用单位数组过滤模式：1（常量YesOrNo.YES）为and过滤（取交集），不传或其他为or过滤（取并集）
        /// </summary>
        public string UseOrgKeyListFilterMode { get; set; }

        /// <summary>
        /// 是否财司代理户
        /// </summary>
        public string IsFinanceProxy { get; set; }

        /// <summary>
        /// 查内部户时的银行账号ID
        /// </summary>
        public string BankAcctKeyForInnerAcct { get; set; }

        /// <summary>
        /// 内部户账户类别
        /// </summary>
        public string InnerAcctCategory { get; set; }

        /// <summary>
        /// 内部户是否为往来账户(内部户账户类别中设置) 1是 2否
        /// </summary>
        public string ReciprocalNot { get; set; }

        /// <summary>
        /// 单位编号模糊查询
        /// </summary>
        public string OrgCodeLike { get; set; }

        /// <summary>
        /// 单位名称模糊查询
        /// </summary>
        public string OrgNameLike { get; set; }

        /// <summary>
        /// 是否排除币种/单位等明细信息  1 是
        /// </summary>
        public string ExcludeDetailInfo { get; set; }

        /// <summary>
        /// 是否银行虚拟户
        /// </summary>
        public string VirtualAcctStatus { get; set; }

        /// <summary>
        /// 是否票据户 1是
        /// </summary>
        public string BillAcctStatus { get; set; }

        /// <summary>
        /// 票据户直联属性
        /// </summary>
        public string BillDirectType { get; set; }

        /// <summary>
        /// 是否直联原因说明
        /// </summary>
        public string AcctInWayReason { get; set; }

        /// <summary>
        /// 是否开通网银原因说明
        /// </summary>
        public string AcctEbankTypeReason { get; set; }

        /// <summary>
        /// 开户区域（基础模块中的国家项）
        /// </summary>
        public string OpenAreaCode { get; set; }

        /// <summary>
        /// 开户区域名称，基础模块中的国家项
        /// </summary>
        public string OpenAreaName { get; set; }

        /// <summary>
        /// 账户开户日期开始
        /// </summary>
        public string AcctOpenDateStart { get; set; }

        /// <summary>
        /// 账户开户日期结束
        /// </summary>
        public string AcctOpenDateEnd { get; set; }

        /// <summary>
        /// 账户状态
        /// </summary>
        public string AcctStatus { get; set; }

        /// <summary>
        /// 账户用途代码
        /// </summary>
        public string AcctUseCode { get; set; }

        /// <summary>
        /// 账户用途代码集合
        /// </summary>
        public string AcctUseCodes { get; set; }

        /// <summary>
        /// 忽略冻结类型
        /// </summary>
        public string IngnoreFreezeType { get; set; }

        /// <summary>
        /// 分页标签/下一页标识
        /// </summary>
        public string NextTag { get; set; }
    }


}




