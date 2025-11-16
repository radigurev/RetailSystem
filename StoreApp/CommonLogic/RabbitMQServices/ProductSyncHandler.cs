using Shared.Abstractions;
using Shared.DTOs;

namespace StoreApp.CommonLogic.RabbitMQServices;

/// <summary>
/// Handler when product is synced
/// </summary>
internal class ProductSyncHandler() : IProductSyncHandler
{
    /// <inheritdoc/>
    public Task HandleAsync(
        ProductSyncMessage message, 
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}