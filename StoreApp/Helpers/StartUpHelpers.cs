using Microsoft.EntityFrameworkCore;
using Shared.Messaging;
using StoreApp.Abstractions;
using StoreApp.CommonLogic;
using StoreApp.Database;

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

    public static void AddDbContext(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<StoreDbContext>(options =>
        {
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("StoreConnection"));
        });
    }
}