using Shared.DTOs;

namespace CentralApp.Abstractions;

public interface ICentralToStores
{
    public Task PublishAsync(
        ProductSyncMessage message,
        string routingKey,
        CancellationToken cancellationToken = default);
}