namespace LZ.Model
{

    public class AppliationModel
    {

        /// <summary>
        /// 是否启动mq队列消息
        /// </summary>
        public bool HasMQ { get; set; } = false;
        /// <summary>
        /// 是否注册微信
        /// </summary>
        public bool HasWeixin { get; set; } = false;
        /// <summary>
        /// 是否启用Node
        /// </summary>
        public bool HasNode { get; set; } = false;

        /// <summary>
        /// 模块key
        /// </summary>
        public string ModuleKey { get; set; } = "";

        /// <summary>
        /// 模块标题
        /// </summary>
        public string ModuleTitle { get; set; } = "";
        /// <summary>
        /// 模块标题
        /// </summary>
        public bool HasAddMVC { get; set; } =false;
        /// <summary>
        /// 是否新增ws
        /// </summary>
        public bool HasAddHub{ get; set; } = false;
        /// <summary>
        /// 是否注册微信支付
        /// </summary>
        public bool HasWeiXinPay { get; set; } = false;

        /// <summary>
        /// 注册财资云客户端
        /// </summary>
        public bool HasCzyApipClient { get; set; } = false;
    }
}
