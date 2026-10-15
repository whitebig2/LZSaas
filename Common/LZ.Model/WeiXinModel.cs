namespace LZ.Model
{
    public class WeiXinModel
    {
        public string Code { get; set; }

        public string AppID { get; set; }

        public string Appsecret { get; set; }

        public string Encrypted { get; set; }

        public string Iv { get; set; }
        public string OpenID { get; set; }
        public string SessionKey { get; set; }
        public string UnionID { get; set; }
        public string Phone { get; set; }
        public string Picture { get; set; }
        public string Name { get; set; }
        public string MPOpenID { get; set; }
        public int? FUID { get; set; }
        public int? UserID { get; set; }
        public int? CompanyID { get; set; }
        public int? CustomerID { get; set; }
        public int? ClientID { get; set; }
    }
}
