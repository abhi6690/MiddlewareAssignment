using System.Text;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;

namespace EventPublisher
{
    public class RabbitMQMessageConsumer : IDisposable
    {
        private readonly RabbitMQConnectionManager _connectionManager;
        private readonly string _exchange;
        private readonly string _exchangeType;
        private readonly string _queueName;

        public RabbitMQMessageConsumer(RabbitMQSetting rabbitMQSetting, string exchange, string exchangeType, string queueName)
        {
            _connectionManager = new RabbitMQConnectionManager(rabbitMQSetting);

            _exchange = exchange;
            _exchangeType = exchangeType;
            _queueName = queueName;

            var channel = _connectionManager.GetChannel();

            channel.ExchangeDeclare(exchange: _exchange, type: _exchangeType);
            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void ConsumeMessages(string routingKey, Action<string> handleMessage)
        {
            var channel = _connectionManager.GetChannel();

            channel.QueueBind(queue: _queueName, exchange: _exchange, routingKey: routingKey);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                handleMessage(message);
            };

            channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        }

        public void Dispose()
        {
            _connectionManager.Dispose();
        }
    }

}
