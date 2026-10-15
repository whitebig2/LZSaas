using RabbitMQ.Client;

namespace LZ.Core.RabbitMQ
{
    public interface IConnection
    {
        global::RabbitMQ.Client.IConnection CreateConnection();

        IModel CreateModel();
    }
}