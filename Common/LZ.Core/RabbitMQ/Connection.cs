using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.RabbitMQ
{
    /// <summary>
    /// 创建信道
    /// </summary>
    public class Connection:IConnection
    {
        private readonly RabbitOptions options;
        
        // 通过
        public Connection(IOptions<RabbitOptions> options)
        {
            this.options = options.Value;
        }
        public global::RabbitMQ.Client.IConnection CreateConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = this.options.HostName,
                Port = this.options.Port,
                DispatchConsumersAsync = true,  //如果使用AsyncEventingBasicConsumer,
                AutomaticRecoveryEnabled = true,
                UserName = this.options.Username,
                Password = this.options.Password,
                VirtualHost = this.options.VirtualHost   //虚拟主机，要在服务器上创建，如果不创建，默认使用"/"，但是不建议使用"/"，因为"/"会和默认的vhost冲突
            };

            // 使用工厂创建连接
            var connection = factory.CreateConnection();
            return connection;
        }


        public IModel CreateModel()
        {
            return this.CreateConnection().CreateModel();
        }
    }
}
