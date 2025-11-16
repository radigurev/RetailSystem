using CentralApp.Models;
using Shared.Abstractions.Caches;
using Shared.Exceptions;
using Shared.Messaging;

using static CentralApp.Helpers.CentralConstants;

namespace CentralApp.CommonLogic.RabbitMQ;

/// <summary>
/// Implementation of RabbitMQConsumer for Central
/// </summary>
/// <param name="_mqService"></param>
/// <param name="_logger"></param>
/// <param name="_scopeFactory"></param>
/// <param name="_cacheService"></param>
public class CentralRabbitMQConsumer(
    IMqService _mqService, 
    ILogger<RabbitMQConsumer> _logger, 
    IServiceScopeFactory _scopeFactory) 
    : RabbitMQConsumer(_mqService, _logger, _scopeFactory)
{
    protected override async Task<string> GetQueueName(ICacheService cacheService)
    {
        ConfigDTO? config = await cacheService.GetByKeyAsync<ConfigDTO, string>(QUEUE_NAME_KEY);

        return config is null ? throw new ConfigMissingException($"Configuration with key {QUEUE_NAME_KEY} is missing", QUEUE_NAME_KEY) : config.Value;
    }
}