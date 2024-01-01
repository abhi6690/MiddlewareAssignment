using EventPublisher;

namespace NotificationService2
{
    public class OrderCreationEventConsumer : BackgroundService
    {
        private readonly ILogger<OrderCreationEventConsumer> _logger;
        private readonly RabbitMQMessageConsumer _messageConsumer;

        public OrderCreationEventConsumer(ILogger<OrderCreationEventConsumer> logger, RabbitMQSetting rabbitMQSetting)
        {
            _logger = logger;
            _messageConsumer = new RabbitMQMessageConsumer(rabbitMQSetting, "order_create_exchange", "fanout", "order_create_queue");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _messageConsumer.ConsumeMessages("creation",
                (message) => _logger.LogInformation($"Notification service 2: Received order creation notification message - {message}"));
            await Task.CompletedTask;
        }
    }
}
