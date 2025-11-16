using CentralApp.Abstractions;
using CentralApp.CommonLogic;
using CentralApp.CommonLogic.Producers;
using CentralApp.Database;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Shared.Abstractions;
using Shared.ExceptionHandlers;
using Shared.Messaging;

namespace CentralApp.Extensions;

public static class StartUpExtensions
{
    /// <param name="builder"></param>
    extension(IHostApplicationBuilder builder)
    {
        /// <summary>
        /// Sets Db context connection and settings
        /// </summary>
        public void AddDbContext()
        {
            builder.Services.AddDbContext<CentralDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("StoreConnection"));
            });
        }

        /// <summary>
        /// Adds DI to api
        /// </summary>
        public void AddServices()
        {
            builder.Services.AddSingleton<IReadOnlyList<ExchangeDeclareDTO>>(
            [
                new ExchangeDeclareDTO("central-sync", ExchangeType.Fanout, true, false)
            ]);
        
            builder.Services.AddSingleton<IProductSyncHandler, ProductSyncHandler>();   
            builder.Services.AddSingleton<IMqService, RabbitMqService>();
            builder.Services.AddHostedService<RabbitMqHostedService>();
            builder.Services.AddHostedService<RabbitMQConsumer>();
        
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IStoreService, StoreService>();
        }

        /// <summary>
        /// Adds Exception handlers for HTTP responses
        /// </summary>
        public void AddExceptionHandlers()
        {
            builder.Services.AddExceptionHandler<EntityNotFoundExceptionHandler>();
        }
    }
}