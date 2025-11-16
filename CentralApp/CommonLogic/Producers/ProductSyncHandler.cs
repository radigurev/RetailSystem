using Shared.Abstractions;
using Shared.DTOs;

namespace CentralApp.CommonLogic.Producers;

/// <summary>
/// Handles what happens with products coming from MQ
/// </summary>
/// <param name="logger"></param>
internal class ProductSyncHandler(ILogger<ProductSyncHandler> logger) : IProductSyncHandler
{
    /// <inheritdoc/>
    public Task HandleAsync(ProductSyncMessage message, CancellationToken cancellationToken)
    {
        // TODO: Upsert product in central DB here.

        logger.LogInformation(
            "Processing product sync in handler. Store={StoreId}, Action={Type}, Product={ProductId}",
            message.StoreId,
            message.Type,
            message.Product.Id);

        return Task.CompletedTask;
    }
}