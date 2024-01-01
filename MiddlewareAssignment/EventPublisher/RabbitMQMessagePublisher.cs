using System.Text;
using RabbitMQ.Client;

namespace EventPublisher
{
    public class RabbitMQMessagePublisher : IDisposable
    {
        private readonly RabbitMQConnectionManager _connectionManager;
        private readonly string _exchange;
        private readonly string _exchangeType;
        private readonly string _queueName;

        public RabbitMQMessagePublisher(RabbitMQSetting rabbitMQSetting, string exchange, string exchangeType, string queueName)
        {
            _connectionManager = new RabbitMQConnectionManager(rabbitMQSetting);
            var channel = _connectionManager.GetChannel();

            _exchange = exchange;
            _exchangeType = exchangeType;
            _queueName = queueName;

            channel.ExchangeDeclare(exchange: _exchange, type: _exchangeType);
            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void PublishMessage(string message, string routingKey)
        {
            var channel = _connectionManager.GetChannel();

            channel.QueueBind(queue: _queueName, exchange: _exchange, routingKey: routingKey);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: _exchange, routingKey: routingKey, basicProperties: null, body: body);
            Console.WriteLine($"Sent '{message}' with routing key '{routingKey}'");
        }

        public void Dispose()
        {
            _connectionManager.Dispose();
        }
    }
}
