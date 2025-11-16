using Shared.Abstractions.Caches;
using Shared.Exceptions;
using Shared.Messaging;
using StoreApp.Models;
using static StoreApp.Helpers.StoreConstants;

namespace StoreApp.CommonLogic.RabbitMQServices;

/// <summary>
/// Rabbit MQ Consumer with local Configuration
/// </summary>
/// <param name="_mqService"></param>
/// <param name="_logger"></param>
/// <param name="_scopeFactory"></param>
/// <param name="_cacheService"></param>
public class StoreRabbitMQConsumer (
        IMqService _mqService, 
        ILogger<StoreRabbitMQConsumer> _logger,
        IServiceScopeFactory _scopeFactory) 
    : RabbitMQConsumer(_mqService, _logger, _scopeFactory)
{
    protected override async Task<string> GetQueueName(ICacheService cacheService)
    {
        ConfigDTO? config = await cacheService.GetByKeyAsync<ConfigDTO, string>(QUEUE_NAME_KEY);
        
        return config is null 
            ? throw new ConfigMissingException($"Configuration with key {QUEUE_NAME_KEY} is missing", QUEUE_NAME_KEY) 
            : config.Value;
    }
}