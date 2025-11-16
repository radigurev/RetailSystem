using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Shared.Abstractions.Caches;
using Shared.DTOs;
using Shared.Exceptions;
using Shared.Messaging;
using StoreApp.Abstractions;
using StoreApp.Models;
using static StoreApp.Helpers.StoreConstants;

namespace StoreApp.CommonLogic.RabbitMQServices;

/// <summary>
/// Implementation of Store communication to RabbitMQ
/// </summary>
/// <param name="mqService"></param>
public class StoreToCentral(
    IMqService _mqService,
    ICacheService _cacheService) : IStoreToCentral
{
    
    /// <inheritdoc/>
    public async Task PublishAsync(
        ProductSyncMessage message,
        CancellationToken cancellationToken = default)
    {
        await _mqService.WaitUntilReadyAsync(cancellationToken);

        string json = JsonSerializer.Serialize(message);
        byte[] body = Encoding.UTF8.GetBytes(json);
        
        ConfigDTO? config = await _cacheService.GetByKeyAsync<ConfigDTO, string>(CENTRAL_SYNC_KEY, cancellationToken);
        ConfigDTO? configRouting = await _cacheService.GetByKeyAsync<ConfigDTO, string>(CENTRAL_EXCHANGE_ROUTING_KEY, cancellationToken);
        
        if(config is null)
            throw new ConfigMissingException("Missing config", CENTRAL_SYNC_KEY);
        
        await _mqService.Channel.BasicPublishAsync(
            exchange: config.Value,
            routingKey: configRouting?.Value ?? "",
            mandatory: false,
            body: body,
            cancellationToken: cancellationToken);
    }
}