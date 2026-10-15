namespace LZ.Model.ApiModel
{
    public class JuMeiResultModel
    {
        public int code { get; set; }
        public string taskNo { get; set; } = string.Empty;
        public string msg { get; set; } = string.Empty;
        public bool success { get; set; }
    }
}
