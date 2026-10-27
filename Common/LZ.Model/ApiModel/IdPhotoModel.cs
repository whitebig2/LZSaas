namespace LZ.Model.ApiModel
{
    public class IdPhotoModel : JuMeiResultModel
    {
        public IdPhotoModelResultModel? data { get; set; }
    }
    public class IdPhotoModelResultModel
    {
        public string result { get; set; }
        public string base64 { get; set; }
        public int[] size { get; set; }
    }
}
