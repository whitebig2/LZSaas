namespace LZ.Model
{
    /// <summary>
    /// 请求结果返回实体
    /// </summary>
    [Serializable]
    public class RequestResultObject<T>
    {
        public RequestResultObject()
        {

        }

        public RequestResultObject(int code, string message, T data=default)
        {
            Code = code;
            Message = message;
            Data = data;
        }

        /// <summary>
        /// 状态码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public long Timestamp { get; set; } = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }

}
