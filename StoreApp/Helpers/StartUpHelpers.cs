using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Shared.Abstractions.Caches;
using Shared.CommonLogic;
using Shared.ExceptionHandlers;
using Shared.Exceptions;
using Shared.Messaging;
using StoreApp.Abstractions;
using StoreApp.CommonLogic;
using StoreApp.CommonLogic.Caches;
using StoreApp.CommonLogic.DatabaseServices;
using StoreApp.CommonLogic.RabbitMQServices;
using StoreApp.Database;
using StoreApp.Models;

namespace StoreApp.Helpers;

/// <summary>
/// Extensions to help readability for Program.cs
/// </summary>
public static class StartUpHelpers
{
    /// <param name="builder"></param>
    extension(IHostApplicationBuilder builder)
    {
        /// <summary>
        /// Add Services for DI
        /// </summary>
        public void AddServices()
        {
            builder.Services.AddFusionCache();

            builder.Services.AddSingleton<IReadOnlyList<ExchangeDeclareDTO>>(
            [
                new ExchangeDeclareDTO("central-sync", ExchangeType.Fanout, true, false)
            ]);
            builder.Services.AddSingleton<IMqService, RabbitMqService>();
            
            builder.Services.AddScoped<ICacheService, CacheService>();
            builder.Services.AddScoped<ICacheDataSourceFactory, CacheDataSourceFactory>();
            builder.Services.AddScoped<ICacheDataSource<ConfigDTO, string>, ConfigCacheService>();
            builder.Services.AddScoped<IStoreToCentral, StoreToCentral>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IConfigService, ConfigService>();
            
            builder.Services.AddHostedService<RabbitMqHostedService>();
            builder.Services.AddHostedService<StoreRabbitMQConsumer>();

        }

        /// <summary>
        /// Adds all exception handlers
        /// </summary>
        public void AddExceptionHandlers()
            => builder.Services.AddExceptionHandler<EntityNotFoundExceptionHandler>();

        /// <summary>
        /// Sets Db context connection and settings
        /// </summary>
        public void AddDbContext()
        {
            builder.Services.AddDbContext<StoreDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("StoreConnection"));
            });
        }
    }
}