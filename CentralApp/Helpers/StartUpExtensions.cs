using CentralApp.Abstractions;
using CentralApp.CommonLogic.Caches;
using CentralApp.CommonLogic.DatabaseServices;
using CentralApp.CommonLogic.RabbitMQ;
using CentralApp.Database;
using CentralApp.Models;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Shared.Abstractions;
using Shared.Abstractions.Caches;
using Shared.CommonLogic;
using Shared.ExceptionHandlers;
using Shared.Messaging;

namespace CentralApp.Helpers;

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
                    builder.Configuration.GetConnectionString("CentralConnection"));
            });
        }

        /// <summary>
        /// Adds DI to api
        /// </summary>
        public void AddServices()
        {
            builder.Services.AddFusionCache();
            
            builder.Services.AddSingleton<IReadOnlyList<ExchangeDeclareDTO>>(
            [
                new ExchangeDeclareDTO("central-sync", ExchangeType.Fanout, true, false)
            ]);

            builder.Services.AddSingleton<IMqService, RabbitMqService>();
            
            builder.Services.AddScoped<IConfigService, ConfigService>();
            builder.Services.AddScoped<ICacheService, CacheService>();
            builder.Services.AddScoped<ICacheDataSourceFactory, CacheDataSourceFactory>();
            builder.Services.AddScoped<ICacheDataSource<ConfigDTO, string>, ConfigCacheService>();
            builder.Services.AddScoped<IProductSyncHandler, ProductSyncHandler>();
            builder.Services.AddScoped<IStoreService, StoreService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICentralToStores, CenterToStores>();
            builder.Services.AddHostedService<RabbitMqHostedService>();
            builder.Services.AddHostedService<CentralRabbitMQConsumer>();
        }

        /// <summary>
        /// Adds Exception handlers for HTTP responses
        /// </summary>
        public void AddExceptionHandlers()
        {
            builder.Services
                .AddExceptionHandler<EntityNotFoundExceptionHandler>()
                .AddExceptionHandler<ConfigMissingExceptionHandler>();
        }
    }
}