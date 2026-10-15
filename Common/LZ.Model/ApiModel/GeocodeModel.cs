namespace LZ.Model.ApiModel
{
    public class GeocodeModel : JuMeiResultModel
    {
        public GeocodeModelResultModel? data { get; set; }
    }
    public class GeocodeModelResultModel
    {
        public string result { get; set; }
        public string base64 { get; set; }
        public int[] size { get; set; }
    }
}
