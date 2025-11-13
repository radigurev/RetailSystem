using Shared.Messaging;
using StoreApp.Abstractions;
using StoreApp.CommonLogic;

namespace StoreApp.Extensions;

public static class StartUpHelpers
{
    public static void AddServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IMqService, RabbitMqService>();
        
        builder.Services.AddScoped<IStoreToCentral, StoreToCentral>();
        
        builder.Services.AddHostedService<RabbitMqHostedService>();
    }
}