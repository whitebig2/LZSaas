using LZ.Model.Enums;

namespace LZ.Model
{
    public class UserToken
    {
        public int UserID { get; set; }
        public string Token { get; set; }
        public string IPAddress { get; set; }
        public string UserAgent { get; set; }
        public DateTime EndTime { get; set; }
        public PlatformType PlatformType { get; set; }
    }
}
