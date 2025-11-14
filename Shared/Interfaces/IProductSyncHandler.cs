using Shared.DTOs;

namespace CentralApp.Services.Producers;

public interface IProductSyncHandler
{
    Task HandleAsync(ProductSyncMessage message, CancellationToken cancellationToken);
}