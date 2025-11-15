using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Shared.DTOs;
using Shared.Messaging;
using StoreApp.Abstractions;

namespace StoreApp.CommonLogic;

/// <summary>
/// Implementation of Store communication to RabbitMQ
/// </summary>
/// <param name="mqService"></param>
public class StoreToCentral(IMqService mqService) : IStoreToCentral
{
    private readonly IMqService _mqService = mqService; 
    
    /// <inheritdoc/>
    public async Task PublishAsync(
        ProductSyncMessage message,
        CancellationToken cancellationToken = default)
    {
        await _mqService.WaitUntilReadyAsync(cancellationToken);

        string json = JsonSerializer.Serialize(message);
        byte[] body = Encoding.UTF8.GetBytes(json);
        
        await _mqService.Channel.BasicPublishAsync(
            exchange: "central-sync",
            routingKey: string.Empty,
            mandatory: false,
            body: body,
            cancellationToken: cancellationToken);
    }
}