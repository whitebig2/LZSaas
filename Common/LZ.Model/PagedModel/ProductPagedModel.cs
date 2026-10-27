namespace LZ.Model.PagedModel
{
    public class ProductPagedModel : PagedModelBase
    {

        /// <summary>
        /// 
        /// </summary>
        public int? Status { get; set; }
        public int? Type { get; set; }
        public int? ShopID { get; set; }
        public Guid? ShopGID { get; set; }
        public Guid? PromotionGID { get; set; }
        public int? PromotionID { get; set; }
        public int? ProductType { get; set; }
        public string Tab { get; set; }
        public string ProductIds { get; set; }
        public bool Expire { get; set; }
        public int? CouponID { get; set; }
        public int? ProductID { get; set; }
        public int? Score { get; set; }
        public string OrderID { get; set; }
        public bool HasPicture { get; set; }
    }
}
