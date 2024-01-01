using EventPublisher;

namespace NotificationService2
{
    public class OrderUpdationEventConsumer : BackgroundService
    {
        private readonly ILogger<OrderUpdationEventConsumer> _logger;
        private readonly RabbitMQMessageConsumer _messageConsumer;

        public OrderUpdationEventConsumer(ILogger<OrderUpdationEventConsumer> logger, RabbitMQSetting rabbitMQSetting)
        {
            _logger = logger;
            _messageConsumer = new RabbitMQMessageConsumer(rabbitMQSetting, "order_update_exchange", "topic", "order_update_queue");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _messageConsumer.ConsumeMessages("updation",
                (message) => _logger.LogInformation($"Notification service 2: Received order updation notification message - {message}"));
            await Task.CompletedTask;
        }
    }
}
