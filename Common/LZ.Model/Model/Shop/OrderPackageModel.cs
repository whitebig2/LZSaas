using SqlSugar;

namespace LZ.Model
{
    public class OrderPackageModel : Shop_OrderPackage
    {
        [SugarColumn(IsIgnore = true)]
        public List<Shop_V_OrderPackageItems> Items { get; set; } = new List<Shop_V_OrderPackageItems>();
    }
}
