using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.Wallet
{
    //《融e聚钱包电子账户销户发短信接口》
    public class BankWalletAcctClose
    {
        /// <summary>
        /// 返回码，交易成功返回0	
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 返回码说明
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 消息号
        /// </summary>
        public string MsgID { get; set; }
        /// <summary>
        /// 合作方交易单号
        /// </summary>
        public string CorpSerno { get; set; }
        /// <summary>
        /// 工行交易订单号
        /// </summary>
        public string EventNo { get; set; }
        /// <summary>
        /// 短信编码
        /// </summary>
        public string SmsSendNo { get; set; }
    }

    //《融e聚钱包电子账户销户验证码校验接口》
    public class BankWalletAcctCloseMsgCheck
    {
        /// <summary>
        /// 返回码，交易成功返回0	
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 返回码说明
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 消息号
        /// </summary>
        public string MsgID { get; set; }
        /// <summary>
        /// 合作方交易单号
        /// </summary>
        public string CorpSerno { get; set; }
        /// <summary>
        /// 工行交易订单号
        /// </summary>
        public string EventNo { get; set; }
    }

    //《融e聚电子钱包账户开立通用页面接口结果查询V1》

    public class BankWalletAcctQuery
    {
        /// <summary>
        /// 返回码，交易成功返回0	
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 返回码说明
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 消息号
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 外部用户编号
        /// </summary>
        public string OutUserId { get; set; }
        /// <summary>
        /// 开户状态。00-初始；01-开户中；02-开户成功；03-开户失败
        /// </summary>
        public string OpenacctStatus { get; set; }
        /// <summary>
        /// 融e聚电子账户账号
        /// </summary>
        public string MediumId { get; set; }
        /// <summary>
        /// 开户失败错误码
        /// </summary>
        public string OpenAcctErrorCode { get; set; }
        /// <summary>
        /// 开户失败错误原因
        /// </summary>
        public string OpenAcctErrorMsg { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string CustName { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string CertNo { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string MobileNo { get; set; }
        /// <summary>
        /// 协议开通类型 1:【小额免密】 2:【新型无卡】
        /// </summary>
        public string ProtocolType { get; set; }
        /// <summary>
        /// 签约协议状态 0-未签约, 1-已签约
        /// </summary>
        public string MediumProStatus { get; set; }
        /// <summary>
        /// 签约协议编号
        /// </summary>
        public string MediumProId { get; set; }

    }

}
