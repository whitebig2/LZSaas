namespace LZ.Model.ApiModel
{
    public class BankImageModel : BasicImageModel
    {
        public BankImageResultModel? result { get; set; }
    }
    public class BankImageResultModel
    {
        public string card_number { get; set; } = string.Empty;
        public string bank_name { get; set; } = string.Empty;
        public string bank_identification_number { get; set; } = string.Empty;
        public string card_name { get; set; } = string.Empty;
        public string card_type { get; set; } = string.Empty;
        public string image { get; set; } = string.Empty;
    }
}
