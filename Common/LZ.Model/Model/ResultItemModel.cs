namespace LZ.Model
{
    public class ResultItemModel<T>
    {

        /// <summary>
        /// 状态码
        /// </summary>
        public int Count { get; set; }


        /// <summary>
        /// 返回数据
        /// </summary>
        public T Items { get; set; }

    }
}
