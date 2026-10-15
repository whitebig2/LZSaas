using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum IntegralType
    {
        /// <summary>
        /// 新员工注册
        /// </summary>
        [Description("新员工注册")]
        Register = 1,
        /// <summary>
        /// 推荐奖励
        /// </summary>
        [Description("推荐奖励")]
        Recommend = 2,
        /// <summary>
        /// 分享岗位
        /// </summary>
        [Description("分享岗位")]
        ShareJob = 3,
        /// <summary>
        /// 入职积分
        /// </summary>
        [Description("入职积分")]   
        Entry = 4,
        /// <summary>
        /// 入职满一月积分
        /// </summary>
        [Description("入职满一月积分")]
        EntryMonth = 5,
        /// <summary>
        /// 推荐新员工入职积分
        /// </summary>
        [Description("推荐新员工入职积分")]
        RecommendEntry = 6,
        /// <summary>
        /// 商城消费
        /// </summary>
        [Description("商城消费")]
        Consume = 7,
        /// <summary>
        /// 取消消费
        /// </summary>
        [Description("取消消费")]
        CancelConsume = 8,
        /// <summary>
        /// 签到积分
        /// </summary>
        [Description("签到积分")]
        Sign = 9,
        /// <summary>
        /// 平台充值
        /// </summary>
        [Description("平台充值")]
        Recharge = 10,
        /// <summary>
        /// 消费返积分
        /// </summary>
        [Description("消费返积分")]
        ConsumeReturn = 11,
        /// <summary>
        /// 中奖赠送积分
        /// </summary>
        [Description("中奖赠送积分")]
        Lottery = 12,
        /// <summary>
        /// 服务商赠送
        /// </summary>
        [Description("服务商赠送")]
        PartnerGift = 13,
        /// <summary>
        /// 服务商充值
        /// </summary>
        [Description("服务商充值")]
        PartnerRecharge = 14,
        /// <summary>
        /// 岗位充值
        /// </summary>
        [Description("岗位充值")]
        JobRecharge = 15,
        /// <summary>
        /// 公司采购
        /// </summary>
        [Description("公司采购")]
        Company = 16
    }
}
