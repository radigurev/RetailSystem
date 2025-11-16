using System.Text;
using System.Text.Json;
using CentralApp.Abstractions;
using RabbitMQ.Client;
using Shared.DTOs;
using Shared.Messaging;

using static CentralApp.Helpers.CentralConstants;

namespace CentralApp.CommonLogic.RabbitMQ;

public class CenterToStores(IMqService _mqService) : ICentralToStores
{
    public async Task PublishAsync(
        ProductSyncMessage message, 
        CancellationToken cancellationToken = default)
    {
        await _mqService.WaitUntilReadyAsync(cancellationToken);

        string json = JsonSerializer.Serialize(message);
        byte[] body = Encoding.UTF8.GetBytes(json);
        
        await _mqService.Channel.BasicPublishAsync(
            exchange: STORE_EXCHANGE_KEY,
            routingKey: STORE_EXCHANGE_ROUTING_KEY,
            mandatory: false,
            body: body,
            cancellationToken: cancellationToken);    }
}