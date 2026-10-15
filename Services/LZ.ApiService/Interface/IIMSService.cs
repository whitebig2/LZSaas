using LZ.Model;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace LZ.ApiService.Interface
{
    public interface IIMSService
    {
        /// <summary>
        /// 发送即时通讯信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<string> SendMessage(Model.IMMessage model);
        /// <summary>
        /// 加入群组
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="type"></param>
        /// <param name="cid"></param>
        /// <returns></returns>
        Task<string> JoinGroup(int uid, int type, int cid);
        /// <summary>
        /// 批量加入群组
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<string> BatchJoinGroup(int uid, int type);
    }
}
