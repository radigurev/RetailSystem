using Shared.DTOs;

namespace CentralApp.Services.Producers;

internal class ProductSyncHandler(ILogger<ProductSyncHandler> logger) : IProductSyncHandler
{
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