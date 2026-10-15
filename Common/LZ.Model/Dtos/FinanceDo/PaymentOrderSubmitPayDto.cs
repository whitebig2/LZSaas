using LZ.Model.Enums;

namespace LZ.Model.Dtos.FinanceDo
{
    public class PaymentOrderSubmitPayDto
    {
        public Guid ID { get; set; }
    
        /// <summary>
        /// TOTP 支付令牌
        /// </summary>
        public string TotpCode { get; set; }
        public byte[] Timestamp { get; set; }
    }
}
