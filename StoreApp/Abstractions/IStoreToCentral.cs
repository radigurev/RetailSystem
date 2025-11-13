using Shared.DTOs;

namespace StoreApp.Abstractions;

public interface IStoreToCentral
{
    public Task PublishAsync(ProductSyncMessage message, CancellationToken cancellationToken = default);
}