namespace LZ.Model.ApiModel
{
    public class BankInfoModel: JuMeiResultModel
    {
        public BankInfoResultModel? data { get; set; }
    }
    public class BankInfoResultModel
    {
        public string bank { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string card_name { get; set; }
        public string tel { get; set; }
        public string type { get; set; }
        public string logo { get; set; }
        public string abbreviation { get; set; }
        public string card_bin { get; set; }
        public int bin_digits { get; set; }
        public int card_digits { get; set; }
        public bool isLuhn { get; set; }
        public string weburl { get; set; }
    }
}
