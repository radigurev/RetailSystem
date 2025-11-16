using Shared.DTOs;

namespace CentralApp.Abstractions;

public interface ICentralToStores
{
    public Task PublishAsync(
        ProductSyncMessage message,
        CancellationToken cancellationToken = default);
}