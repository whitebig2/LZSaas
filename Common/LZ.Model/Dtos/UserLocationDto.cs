namespace LZ.Model.Dtos
{
    /// <summary>
    /// 小程序进入时上报的实时位置（用于人岗匹配的距离分档与画像展示）。
    /// 由小程序端定位授权后调用，每次进入小程序上报一次，覆盖上一次位置。
    /// </summary>
    public class UserLocationDto
    {
        /// <summary>
        /// 省份（如"江苏省"，小程序定位解析）
        /// </summary>
        public string MiniProvince { get; set; } = string.Empty;

        /// <summary>
        /// 城市（如"苏州市"）
        /// </summary>
        public string MiniCity { get; set; } = string.Empty;

        /// <summary>
        /// 区域（如"吴中区"）
        /// </summary>
        public string MiniDistrict { get; set; } = string.Empty;

        /// <summary>
        /// 详细地址（如"苏州市吴中区金枫路xxx号"）
        /// </summary>
        public string MiniAddress { get; set; } = string.Empty;

        /// <summary>
        /// 纬度（-90 ~ 90）
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// 经度（-180 ~ 180）
        /// </summary>
        public double? Longitude { get; set; }
    }
}
