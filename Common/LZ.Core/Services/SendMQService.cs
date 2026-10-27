using LZ.Common;
using LZ.Core.Attributes;
using LZ.Core.EventData;
using LZ.Core.EventData.Insurance;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
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
    public class SendMQService(IEventPublisher publisher) : ISendMQService, IScopedDependency
    {
        public void SendStatisticsMessage(string json)
        {
            var message = new SendMessageEventData();//发送商保是否增减员
            message.From = "statistics";
            message.Json = json;
            publisher.Publish(message);
        }
    }
}
