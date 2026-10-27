using LZ.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    public class CompanyConfig
    {
        /// <summary>
        /// 员工入职后是否可以查看岗位
        /// </summary>
        public bool UserIsShowJob { get; set; }
        /// <summary>
        /// 已在职的员工是否能无缝入职到其他客户
        /// </summary>
        public bool UserIsEntryOther { get; set; }
        /// <summary>
        /// 员工扫码后已存在待面试或待处理中的记录是否覆盖
        /// </summary>
        public bool UserIsEntryCover { get; set; }
        /// <summary>
        /// 员工扫码后是否到待入职
        /// </summary>
        public bool UserIsToWaitEntry { get; set; }
        /// <summary>
        /// 是否由驻厂负责离职
        /// </summary>
        public bool UserIsFactoryLeave { get; set; }
        /// <summary>
        /// 是否使用公共黑名单
        /// </summary>
        public bool UserIsUsePublicBlcak { get; set; }
        /// <summary>
        /// 扫码后是否填写个人简介
        /// </summary>
        public bool UserIsInputResume { get; set; }
        /// <summary>
        /// 是否要关联公众号
        /// </summary>
        public bool UserIsAssociationMP { get; set; }
        /// <summary>
        /// 是否生成公众号二维码
        /// </summary>
        public bool UserIsMPQrcode { get; set; }
        /// <summary>
        /// 蓝领通过幸福职公众号发送通知
        /// </summary>
        public bool UserIsXFZMPNotice { get; set; }
        /// <summary>
        /// 员工电子证件照的SpecID
        /// </summary>
        public int UserIdPhontSpecID { get; set; } = 18;
        /// <summary>
        /// 员工电子证件照的背景颜色
        /// </summary>
        public string UserIdPhontColor { get; set; } = "blue";
        /// <summary>
        /// 员工电子证件照的美颜
        /// </summary>
        public decimal UserIdPhontBeautyDegree { get; set; } = 1.5M;
        /// <summary>
        /// 是否启动联营模式
        /// </summary>
        public bool CustomerIsJoint{ get; set; }
        /// <summary>
        /// 是否启动平台联营
        /// </summary>
        public bool CustomerIsPlatformJoint { get; set; }
        /// <summary>
        /// 是否启动申请联营审核
        /// </summary>
        public bool CustomerIsJointAudit { get; set; }
        /// <summary>
        /// 联营人数
        /// </summary>
        public int CustomerJointCount { get; set; }
        /// <summary>
        /// 联营人数未达标是否再次开启联营
        /// </summary>
        public bool CustomerIsJointAgain { get; set; }
        /// <summary>
        /// 员工实名认证方式
        /// </summary>
        public RealAuthModel RealAuthModel { get; set; }
        /// <summary>
        /// 在岗位页面是使用即时通讯还是手机号
        /// </summary>
        public bool UserIsJobUseIM { get; set; }
        /// <summary>
        /// 客服人员ID
        /// </summary>
        public int UserCustomerServiceID { get; set; }
        /// <summary>
        /// 客服人员显示的名称
        /// </summary>
        public string UserCustomerServiceName { get; set; }
        /// <summary>
        /// 是否正式使用系统
        /// </summary>
        public bool CompanyIsFormal { get; set; }
        /// <summary>
        /// 入职时间能否修改
        /// </summary>
        public bool UserIsEntryLeaveDateEdit { get; set; }
        /// <summary>
        /// 不能办理入职时间段
        /// </summary>
        public string NoEntryTimes { get; set; }
        /// <summary>
        /// 公司logo
        /// </summary>
        public string CompanyLogo { get; set; }
        /// <summary>
        /// 是否在办理入职的时候修改招聘专员
        /// </summary>
        public bool EntryChangePerUser { get; set; }
        /// <summary>
        /// 是否在后台客户显示盖章单位
        /// </summary>
        public bool AdminIsShowCustomerCompany { get; set; }
        /// <summary>
        /// 生成二维码的类型
        /// </summary>
        public CreateQrcodeType CreateQrcodeType { get; set; }
        /// <summary>
        /// 幸福职小程序是否只展示联营岗位
        /// </summary>
        public bool UserIsOnlyJoin { get; set; }

        /// <summary>
        /// 黑名单是否禁止入职
        /// </summary>
        public bool IsBlackNoEntry { get; set; }
        /// <summary>
        /// 新增线下员工手机号是否必填
        /// </summary>
        public bool IsOfflinePhone { get; set; }

        /// <summary>
        /// 是否启动多驻厂，根据招聘渠道区分
        /// </summary>
        public bool IsUseJobChannel { get; set; }
        /// <summary>
        /// 是否启用职喜AI
        /// </summary>
        public bool EnableZhiXiAI { get; set; }
        /// <summary>
        /// 指定快递包装二维码服务商
        /// </summary>
        public int PackregUserID { get; set; }
        /// <summary>
        /// 是否开启岗位审核
        /// </summary>
        public bool EnableJobAudit{ get; set; }
        /// <summary>
        /// 开启对接简道云
        /// </summary>
        public bool EnableDockJDY { get; set; }
        /// <summary>
        /// 简道云api key
        /// </summary>
        public string JDYApiKey { get; set; }
        /// <summary>
        /// 简道云Secret
        /// </summary>
        public string JDYSecret { get; set; }
        /// <summary>
        /// 是否启用投诉
        /// </summary>
        public bool EnableComplaint { get; set; }
        /// <summary>
        /// 投诉电话
        /// </summary>
        public string ComplaintPhone { get; set; }
        /// <summary>
        /// 是否启用客户集团
        /// </summary>
        public bool EnableCustomerGroup { get; set; }
        /// <summary>
        /// 是否启用人岗匹配
        /// </summary>
        public bool EnablePrecomputeJob { get; set; }

        #region 微信
        /// <summary>
        /// 分享岗位说明
        /// </summary>
        public string ShareJobDescription { get; set; }
        /// <summary>
        /// 小程序AppId
        /// </summary>
        public string WeiXinOpenAppId { get; set; }
        /// <summary>
        /// 小程序AppSecret
        /// </summary>
        public string WeiXinOpenAppSecret { get; set; }
        /// <summary>
        /// 微支付密钥
        /// </summary>
        public string WeiXinPaySecret { get; set; }
        /// <summary>
        /// 微支付证书文件路径
        /// </summary>
        public string WeiXinKeyFile { get; set; }
        /// <summary>
        /// 微支付商户ID
        /// </summary>
        public string WeiXinMerchantID { get; set; }
        /// <summary>
        /// 微支付商户名称
        /// </summary>
        public string WeiXinMerchantName { get; set; }

        #endregion

        #region 商城
        /// <summary>
        /// 是否启用商城
        /// </summary>
        public bool IsShop { get; set; }
        /// <summary>
        /// 是否开启转盘
        /// </summary>
        public bool IsTurntable { get; set; }
        /// <summary>
        /// 商城客服人员ID
        /// </summary>
        public int ShopCustomerServiceID { get; set; }
        /// <summary>
        /// 商城客服人员显示的名称
        /// </summary>
        public string ShopCustomerServiceName { get; set; }
        /// <summary>
        /// 转盘次数
        /// </summary>
        public int TurntableCount { get; set; }
        /// <summary>
        /// 是否开启注册
        /// </summary>
        public bool IsRegister { get; set; }
        /// <summary>
        /// 是否启用注册送积分
        /// </summary>
        public bool IsShopRegisterIntegral { get; set; }
        /// <summary>
        /// 新注册用户赠送积分
        /// </summary>
        public int ShopRegisterIntegral { get; set; }
        /// <summary>
        /// 推荐奖励
        /// </summary>
        public int ShopRecommendIntegral { get; set; }
        /// <summary>
        /// 分享岗位积分
        /// </summary>
        public int ShopShareJobIntegral { get; set; }
        /// <summary>
        /// 充值商品的id
        /// </summary>
        public int RechargeProductID { get; set; }
        /// <summary>
        /// 平台积分
        /// </summary>
        public int PlatformIntegral { get; set; }
        #endregion
    }
}
