using System.Text;
using System.Text.Json;
using CentralApp.Abstractions;
using CentralApp.Models;
using RabbitMQ.Client;
using Shared.Abstractions.Caches;
using Shared.DTOs;
using Shared.Exceptions;
using Shared.Messaging;

using static CentralApp.Helpers.CentralConstants;

namespace CentralApp.CommonLogic.RabbitMQ;

public class CenterToStores(
    IMqService _mqService,
    ICacheService _cacheService) : ICentralToStores
{
    public async Task PublishAsync(
        ProductSyncMessage message, 
        string routingKey,
        CancellationToken cancellationToken = default)
    {
        await _mqService.WaitUntilReadyAsync(cancellationToken);

        string json = JsonSerializer.Serialize(message);
        byte[] body = Encoding.UTF8.GetBytes(json);
        
        ConfigDTO? config = await _cacheService.GetByKeyAsync<ConfigDTO, string>(STORE_SYNC_KEY,  cancellationToken);
        
        if(config is null)
            throw new ConfigMissingException("Config not found", STORE_SYNC_KEY);
        
        await _mqService.Channel.BasicPublishAsync(
            exchange: config.Value,
            routingKey: routingKey,
            mandatory: false,
            body: body,
            cancellationToken: cancellationToken);    }
}