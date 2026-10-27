using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class UserEntryExcelTemplate
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [DisplayName("姓名")]
        public string Name { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        [DisplayName("年龄")]
        public int? Age { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        [DisplayName("性别")]
        public string Sex { get; set; }
        /// <summary>
        /// 所属客户
        /// </summary>
        [DisplayName("所属客户")]
        public string CustomerName { get; set; }
        /// <summary>
        /// 所属服务商
        /// </summary>
        [DisplayName("所属服务商")]
        public string PartnerName { get; set; }
        /// <summary>
        /// 招聘专员
        /// </summary>
        [DisplayName("招聘专员")]
        public string PersonnelName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [DisplayName("手机号")]
        public string Phone { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        [DisplayName("身份证号码")]
        public string CardID { get; set; }
        /// <summary>
        /// 工种
        /// </summary>
        [DisplayName("工种")]
        public string WorkName { get; set; }
        /// <summary>
        /// 工种类型
        /// </summary>
        [DisplayName("工种类型")]
        public string WorkClass { get; set; }
        /// <summary>
        /// 招聘渠道
        /// </summary>
        [DisplayName("招聘渠道")]
        public string FromType { get; set; }
        /// <summary>
        /// 入职日期
        /// </summary>
        [DisplayName("入职日期")]
        public string EntryTime { get; set; }
        /// <summary>
        /// 离职日期
        /// </summary>
        [DisplayName("离职日期")]
        public string LeaveTime { get; set; }
        /// <summary>
        /// 签章单位
        /// </summary>
        [DisplayName("签章单位")]
        public string SignName { get; set; }
        /// <summary>
        /// 系统状态
        /// </summary>
        [DisplayName("系统状态")]
        public string StatusName { get; set; }
        /// <summary>
        /// 民族
        /// </summary>
        [DisplayName("民族")]
        public string Nation { get; set; }
        /// <summary>
        /// 婚姻状况
        /// </summary>
        [DisplayName("婚姻状况")]
        public string MaritalStatus { get; set; }
        /// <summary>
        /// 学历
        /// </summary>
        [DisplayName("学历")]
        public string Education { get; set; }
        /// <summary>
        /// 出生年月
        /// </summary>
        [DisplayName("出生年月")]
        public string BirthDate { get; set; }
        /// <summary>
        /// 居住地址
        /// </summary>
        [DisplayName("居住地址")]
        public string Address { get; set; }
        /// <summary>
        /// 户籍地址
        /// </summary>
        [DisplayName("户籍地址")]
        public string IdcardAddress { get; set; }
        /// <summary>
        /// 紧急人姓名
        /// </summary>
        [DisplayName("紧急人姓名")]
        public string EmergencyContact { get; set; }
        /// <summary>
        /// 紧急号码
        /// </summary>
        [DisplayName("紧急号码")]
        public string EmergencyContactPhone { get; set; }
        /// <summary>
        /// 保险状态
        /// </summary>
        [DisplayName("保险状态")]
        public string InsuranceStatus { get; set; }
        /// <summary>
        /// 保险品种
        /// </summary>
        [DisplayName("保险品种")]
        public string PlanName { get; set; }
        /// <summary>
        /// 所在省份
        /// </summary>
        [DisplayName("所在省份")]
        public string CustomerProvince { get; set; }
        /// <summary>
        /// 所在城市
        /// </summary>
        [DisplayName("所在城市")]
        public string CustomerCity { get; set; }
        /// <summary>
        /// 开户行
        /// </summary>
        [DisplayName("开户行")]
        public string BankName { get; set; }
        /// <summary>
        /// 银行卡号
        /// </summary>
        [DisplayName("银行卡号")]
        public string BankNumber { get; set; }
        /// <summary>
        /// 银行卡所属省份
        /// </summary>
        [DisplayName("银行卡所属省份")]
        public string BankProvince { get; set; }
        /// <summary>
        /// 是否实名
        /// </summary>
        [DisplayName("是否实名")]
        public string Real { get; set; }
        /// <summary>
        /// 当月在职天数
        /// </summary>
        [DisplayName("当月在职天数")]
        public int InDays { get; set; }
        /// <summary>
        /// 当月在职天数
        /// </summary>
        [DisplayName("备注")]
        public string Remarks { get; set; }
        /// <summary>
        /// 新增时间
        /// </summary>
        [DisplayName("新增时间")]
        public string CreateTime { get; set; }
        /// <summary>
        /// 便签
        /// </summary>
        [DisplayName("便签")]
        public string Note { get; set; }

        /// <summary>
        /// 主保险公司
        /// </summary>
        [DisplayName("主保险公司")]
        public string InsuranceCompany1 { get; set; }
    }
}
