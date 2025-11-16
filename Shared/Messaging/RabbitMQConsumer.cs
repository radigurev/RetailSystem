using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using Shared.Abstractions;
using Shared.Abstractions.Caches;
using Shared.DTOs;

namespace Shared.Messaging;

/// <summary>
/// Rabbit MQ consumer of incoming messages
/// </summary>
/// <param name="_mqService"></param>
/// <param name="_logger"></param>
public abstract class RabbitMQConsumer(
    IMqService _mqService,
    ILogger<RabbitMQConsumer> _logger,
    IServiceScopeFactory _scopeFactory) : IHostedService
{
    private string? _consumerTag;

    #region  Implementation

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _mqService.WaitUntilReadyAsync(cancellationToken);

        using IServiceScope scope = _scopeFactory.CreateScope();
        ICacheService cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
        string queueName = await GetQueueName(cacheService);

        await _mqService.Channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 10,
            global: false,
            cancellationToken: cancellationToken);

        await _mqService.Channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        await _mqService.Channel.QueueBindAsync(
            queue: queueName,
            exchange: "central-sync",
            routingKey: string.Empty,
            cancellationToken: cancellationToken);

        AsyncEventingBasicConsumer consumer = new(_mqService.Channel);

        consumer.ReceivedAsync += OnReceivedAsync;

        _consumerTag = await _mqService.Channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: false,
            consumerTag: string.Empty,
            noLocal: false,
            exclusive: false,
            arguments: null,
            consumer: consumer,
            cancellationToken: cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(_consumerTag))
        {
            try
            {
                await _mqService.Channel.BasicCancelAsync(
                    consumerTag: _consumerTag,
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error cancelling RabbitMQ consumer on shutdown.");
            }
        }
    }

    #endregion

    private async Task OnReceivedAsync(object sender, BasicDeliverEventArgs ea)
    {
        string deliveryTagInfo = ea.DeliveryTag.ToString();

        try
        {
            byte[] body = ea.Body.ToArray();
            string json = Encoding.UTF8.GetString(body);

            ProductSyncMessage? message =
                JsonSerializer.Deserialize<ProductSyncMessage>(json);

            if (message == null)
            {
                _logger.LogWarning(
                    "Received null ProductSyncMessage. DeliveryTag={Tag}, Payload={Payload}",
                    deliveryTagInfo,
                    json);

                await _mqService.Channel.BasicAckAsync(
                    ea.DeliveryTag,
                    multiple: false);

                return;
            }

            using IServiceScope scope = _scopeFactory.CreateScope();

            IProductSyncHandler handler =
                scope.ServiceProvider.GetRequiredService<IProductSyncHandler>();

            await handler.HandleAsync(message, CancellationToken.None);

            await _mqService.Channel.BasicAckAsync(
                ea.DeliveryTag,
                multiple: false);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error processing message. DeliveryTag={Tag}",
                deliveryTagInfo);

            await _mqService.Channel.BasicNackAsync(
                deliveryTag: ea.DeliveryTag,
                multiple: false,
                requeue: false);
        }
    }

    protected abstract Task<string> GetQueueName(ICacheService cacheService);
}