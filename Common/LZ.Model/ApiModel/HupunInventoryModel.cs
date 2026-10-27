namespace LZ.Model.ApiModel
{
    public class HupunInventoryModel
    {
        public HupunInventoryDataModel data { get; set; }
    }
    public class HupunInventoryDataModel
    {
        public string item_id { get; set; }
        public int quantity { get; set; }
        public string shop_nick { get; set; }
        public string sku_id { get; set; }
    }
}
