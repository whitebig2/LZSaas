using System.ComponentModel;

namespace LZ.Model.IExcelTemplate
{
    public class BusInsuranceApiPushExcelTemplate
    {
        [DisplayName("保险公司")]
        public string InsuranceCompany { get; set; }

        [DisplayName("记录日期")]
        public string RecordDate { get; set; }

        [DisplayName("推送类型")]
        public string PushType { get; set; }

        [DisplayName("状态")]
        public string Status { get; set; }

        [DisplayName("增员人数")]
        public int AddCount { get; set; }

        [DisplayName("减员人数")]
        public int DelCount { get; set; }

        [DisplayName("确认链接")]
        public string ConfirmUrl { get; set; }

        [DisplayName("创建时间")]
        public string CreateTime { get; set; }
    }
}
