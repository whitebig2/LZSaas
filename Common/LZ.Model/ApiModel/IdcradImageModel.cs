namespace LZ.Model.ApiModel
{
    public class IdcradImageModel
    {
        public string msg { get; set; }
        public bool success { get; set; }
        public int code { get; set; }
        public IdcradImageDataModel? data { get; set; }
    }
    public class IdcradImageDataModel
    {
        public string order_no { get; set; }
        public string side { get; set; }
        public IdcradImageResultModel? info { get; set; }
    }
    public class IdcradImageModel_ZS : BasicImageModel
    {
        public IdcradImageResultModel? result { get; set; }
    }
    public class IdcradImageResultModel
    {
        public string name { get; set; } = string.Empty;
        public string sex { get; set; } = string.Empty;
        public string nation { get; set; } = string.Empty;
        public string year { get; set; } = string.Empty;
        public string month { get; set; } = string.Empty;
        public string day { get; set; } = string.Empty;
        public string address { get; set; } = string.Empty;
        public string number { get; set; } = string.Empty;
        public string authority { get; set; } = string.Empty;
        public string timelimit { get; set; } = string.Empty;
        public string image { get; set; } = string.Empty;
    }
}
