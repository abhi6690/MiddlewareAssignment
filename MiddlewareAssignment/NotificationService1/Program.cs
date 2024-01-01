using EventPublisher;
using NotificationService1;

var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";
var configuration = GetConfiguration(environment);
var builder = Host.CreateApplicationBuilder(args);
var rabbitMQSetting = new RabbitMQSetting();
configuration.GetSection("RabbitMQ").Bind(rabbitMQSetting);
builder.Services.AddSingleton(rabbitMQSetting);
builder.Services.AddHostedService<OrderCreationEventConsumer>();

var host = builder.Build();
host.Run();

IConfiguration GetConfiguration(string environment)
{
    return new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{environment}.json", true, true)
        .AddJsonFile($"secrets/appsettings.json", true, true)
        .AddEnvironmentVariables()
        .Build();
}