using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Model
{
    public class CaAuthIdentityViewModel
    {
        [Display(Name = "用户ID")]
        public Guid? Id { get; set; }
        [Display(Name = "用户ID")]
        public Guid? UserGid { get; set; }


        [Required(ErrorMessage = "姓名不能为空")]
        [StringLength(50, ErrorMessage = "姓名长度不能超过50个字符")]
        [Display(Name = "姓名")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "身份证号码不能为空")]
        [RegularExpression(@"^\d{17}[\dXx]$", ErrorMessage = "身份证号码格式不正确")]
        [Display(Name = "身份证号码")]
        public string IdCardNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "手机号码不能为空")]
        [Phone(ErrorMessage = "手机号码格式不正确")]
        [Display(Name = "手机号码")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "请选择认证方式")]
        [Display(Name = "认证方式")]
        public string AuthMethod { get; set; } = "Password"; // Default

        [Display(Name = "认证状态")]
        public bool IsAuthenticated { get; set; }


        [Display(Name = "认证状态")]
        public string StatusText => IsAuthenticated ? "已认证" : "未认证";

        // 用于前端样式类
        public string StatusBadgeClass => IsAuthenticated ? "bg-green-100 text-green-800 border-green-200" : "bg-red-100 text-red-800 border-red-200";

        public Dictionary<string, string> AvailableAuthMethods => new Dictionary<string, string>() { { "Face", "人脸识别" }, { "SMS", "短信验证" } };
        public bool IsApp { get; set; }
        public bool IsSendSms { get; set; }
        [Display(Name = "验证码")]
        public string SmsCode { get; set; }
    }
}
