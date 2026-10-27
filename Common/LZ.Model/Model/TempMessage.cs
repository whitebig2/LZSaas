using System;
using System.Collections.Generic;
using System.Text;

namespace LZ.Model
{
    public class TempMessage
    {
        /// <summary>
        /// 后台用户guid
        /// </summary>
        public Guid? id { get; set; }
        /// <summary>
        /// 相应子账号id
        /// </summary>
        public int uid { get; set; }
        /// <summary>
        /// 后台用户id
        /// </summary>
        public int userid { get; set; }
        public string first { get; set; }
        public string keyword1 { get; set; }
        public string keyword2 { get; set; }
        public string keyword3 { get; set; }
        public string keyword4 { get; set; }
        public string keyword5 { get; set; }
        public string url { get; set; }
        public string remark { get; set; }
        public string openid { get; set; }
        public string templateId { get; set; }
        public string appid { get; set; }
        public string miniappid { get; set; }
        public string type { get; set; }
        public string unionid { get; set; }
        public TempMessage()
        {
            uid = 0;
            userid = 0;
        }
    }
}
