namespace LZ.Model.ApiModel
{
    public class ExpressSubscribeResultModel : JuMeiResultModel
    {
        public object data { get; set; }
    }

    public class ExpressPushModel
    {
        public string expressCode { get; set; }
        public string number { get; set; }
        public string logisticsStatus { get; set; }
        public string logisticsStatusDesc { get; set; }
        public List<ExpressPushDetailModel> details { get; set; }
    }

    public class ExpressPushDetailModel
    {
        public long? time { get; set; }
        public string logisticsStatus { get; set; }
        public string subLogisticsStatus { get; set; }
        public string desc { get; set; }
        public string areaCode { get; set; }
        public string areaName { get; set; }
    }
}
