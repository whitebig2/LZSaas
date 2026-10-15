namespace LZ.Model.ApiModel
{
    public class HupunResultModel
    {
        public int code { get; set; }
        public HupunResultDataModel data { get; set; }
    }
    public class HupunResultDataModel
    {
        public string error_code { get; set; }
        public string error_msg { get; set; }
        public string response { get; set; }
        public bool success { get; set; }
    }
}
