using LZ.Common;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.Enums;
using MathNet.Numerics.Distributions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Senparc.CO2NET;
using SqlSugar;
using SqlSugar.IOC;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace LZ.Core.Services
{
    public class MessageService : Hub
    {
        public override async Task OnConnectedAsync()
        {
            try
            {
                var http = Context.GetHttpContext();
                var q = http.Request.QueryString.Value;
                var reg = new Regex(@"(?<=(\&|\?|^)(from)\=).*?(?=\&|$)");
                var from = reg.Match(q).Value;
                if (string.IsNullOrEmpty(from))
                {
                    return;
                }
                await Groups.AddToGroupAsync(Context.ConnectionId, from);
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
            }
            await base.OnConnectedAsync();
        }
        //发送消息--发送给所有连接的客户端
        public Task SendMessage(string msg)
        {
            return Clients.All.SendAsync("ReceiveMessage", msg);
        }

        //发送消息--发送给指定用户
        public Task SendPrivateMessage(string userId, string msg)
        {
            return Clients.Client(userId).SendAsync("ReceiveMessage", msg);
        }
        /// <summary>
        /// 连接断开的时候调用
        /// </summary>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception ex)
        {
            var http = Context.GetHttpContext();
            var q = http.Request.QueryString.Value;
            var reg = new Regex(@"(?<=(\&|\?|^)(from)\=).*?(?=\&|$)");
            var from = reg.Match(q).Value;
            if (string.IsNullOrEmpty(from))
            {
                return;
            }
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, from);
            await base.OnDisconnectedAsync(ex);
        }
    }
}
