using System.ComponentModel;

namespace LZ.Model.IExcelTemplate
{
    /// <summary>
    /// 商保订单保费清单导入（按订单号分组生成订单及参保方案）
    /// </summary>
    public class BusInsuranceOrderPlanImportExcelTemplate
    {
        [DisplayName("订单号")]
        public string OrderNo { get; set; }

        [DisplayName("工种")]
        public string WorkName { get; set; }

        [DisplayName("被保险人")]
        public string InsuredName { get; set; }

        [DisplayName("保险方案")]
        public string PlanName { get; set; }

        [DisplayName("方案单价")]
        public string PlanPriceAmount { get; set; }
    }
}
