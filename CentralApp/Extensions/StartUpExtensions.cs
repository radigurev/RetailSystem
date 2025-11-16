using CentralApp.Abstractions;
using CentralApp.CommonLogic;
using CentralApp.CommonLogic.Producers;
using CentralApp.Database;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions;
using Shared.ExceptionHandlers;
using Shared.Messaging;

namespace CentralApp.Extensions;

public static class StartUpExtensions
{
    /// <summary>
    /// Sets Db context connection and settings
    /// </summary>
    /// <param name="builder"></param>
    public static void AddDbContext(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<CentralDbContext>(options =>
        {
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("StoreConnection"));
        });
    }
    
    public static void AddServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IProductSyncHandler, ProductSyncHandler>();   
        builder.Services.AddSingleton<IMqService, RabbitMqService>();
        builder.Services.AddHostedService<RabbitMqHostedService>();
        builder.Services.AddHostedService<RabbitMQConsumer>();
        
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IStoreService, StoreService>();
    }

    public static void AddExceptionHandlers(this IHostApplicationBuilder builder)
    {
        builder.Services.AddExceptionHandler<EntityNotFoundExceptionHandler>();
    }
}