using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Exceptions;
using Shared.Messaging;
using StoreApp.Abstractions;
using StoreApp.CommonLogic;
using StoreApp.Database;
using StoreApp.Helpers.ExceptionHandlers;

namespace StoreApp.Helpers;

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
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IConfigService, ConfigService>();
        builder.Services.AddHostedService<RabbitMqHostedService>();
    }

    /// <summary>
    /// Adds all exception handlers
    /// </summary>
    /// <param name="builder"></param>
    public static void AddExceptionHandlers(this IHostApplicationBuilder builder)
        => builder.Services.AddExceptionHandler<EntityNotFoundExceptionHandler>();

    /// <summary>
    /// Sets Db context connection and settings
    /// </summary>
    /// <param name="builder"></param>
    public static void AddDbContext(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<StoreDbContext>(options =>
        {
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("StoreConnection"));
        });
    }
}