using CentralApp.Services;
using CentralApp.Services.Producers;
using Shared.Messaging;

namespace CentralApp.Extensions;

public static class StartUpExtensions
{
    public static void AddServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IProductSyncHandler, ProductSyncHandler>();   
        builder.Services.AddSingleton<IMqService, RabbitMqService>();
        builder.Services.AddHostedService<RabbitMqHostedService>();
        builder.Services.AddHostedService<RabbitMQConsumer>();
    }
}