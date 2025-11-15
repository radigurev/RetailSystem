using Shared.Messaging;
using StoreApp.Abstractions;
using StoreApp.CommonLogic;

namespace StoreApp.Extensions;

/// <summary>
/// Extensions to help readability for Program.cs
/// </summary>
public static class StartUpHelpers
{
    
    /// <summary>
    /// Add Services for DI
    /// </summary>
    /// <param name="builder"></param>
    public static void AddServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IMqService, RabbitMqService>();
        
        builder.Services.AddScoped<IStoreToCentral, StoreToCentral>();
        
        builder.Services.AddHostedService<RabbitMqHostedService>();
    }
}