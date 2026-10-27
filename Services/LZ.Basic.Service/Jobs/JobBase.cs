using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;

namespace LZ.Basic.Service.Jobs
{
    public class JobBase
    {
      //  "RabbitMQ": {
      //  "VirtualHost": "lz-virtual",
      //  "Username": "lz",
      //  "Password": "123456",
      //  "HostName": "106.15.126.71",
      //  "Port": 32770,
      //  "ClientName": "lz-lqm",
      //  "ExchangeName": "lqm_exchange",
      //  "QueueName": "lqm_queue"
      //},
      public string HostName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public string ExchangeName { get; set; }
        public int Port { get; set; }
        public JobBase()
        {
            HostName = "106.15.126.71";
            Username = "lz";
            Password = "123456";
            VirtualHost = "lz-virtual";
            ExchangeName = "lqm_exchange";
            Port = 32770;

        }
        public ConnectionFactory CreateConnectionFactory()
        {
           return  new ConnectionFactory
            {
                HostName = HostName,
                Port = Port,
                DispatchConsumersAsync = true,
                UserName = Username,
                Password = Password,
                VirtualHost = VirtualHost
            };
        }
        public Dictionary<string,string> CreateDictionary(string json)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(json))
            {
                var obj = JsonConvert.DeserializeObject(json) as JArray;
                foreach (var x in obj)
                {
                    dic.Add(x["key"].ToString(), x["value"].ToString());
                }
            }
            return dic;
        }
    }
}
