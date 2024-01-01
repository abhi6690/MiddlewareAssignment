using RabbitMQ.Client;

namespace EventPublisher
{
    public class RabbitMQConnectionManager : IDisposable
    {
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQConnectionManager(RabbitMQSetting rabbitMQSetting)
        {
            _factory = new ConnectionFactory()
            {
                HostName = rabbitMQSetting.HostName,
                UserName = rabbitMQSetting.UserName,
                Password = rabbitMQSetting.Password
            };

            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public IModel GetChannel()
        {
            return _channel;
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }

}
