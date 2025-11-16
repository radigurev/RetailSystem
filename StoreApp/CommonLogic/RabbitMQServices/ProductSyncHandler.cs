using System.Linq.Expressions;
using AutoMapper;
using Shared.Abstractions;
using Shared.DTOs;
using Shared.Enums;
using StoreApp.Abstractions;
using StoreApp.Database.Models;

namespace StoreApp.CommonLogic.RabbitMQServices;

/// <summary>
/// Handler when product is synced
/// </summary>
internal class ProductSyncHandler : IProductSyncHandler
{
    private readonly ILogger<ProductSyncHandler> _logger;
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public ProductSyncHandler(
        ILogger<ProductSyncHandler> logger,
        IProductService productService,
        IMapper mapper)
    {
        _logger = logger;
        _productService = productService;
        _mapper = mapper;
    }
    /// <inheritdoc/>
    public async Task HandleAsync(ProductSyncMessage message, CancellationToken cancellationToken)
    {
        try
        {
            Expression<Func<Product, bool>> productPredicate =
                x => x.Id == message.Product.Id;

            if (message.Type == MQMessageType.Delete)
            {
                await _productService.DeleteAsync(productPredicate, cancellationToken);
                return;
            }
            
            Product toCreate = _mapper.Map<Product>(message.Product);

            await _productService.UpsertProduct(productPredicate, toCreate, cancellationToken);
            
            _logger.LogInformation(
                "Updated central product {ProductName} for store {StoreId}.",
                message.Product.Name,
                message.StoreGuid);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error while syncing product from store {StoreId} product {ProductName}.",
                message.StoreGuid,
                message.Product.Name);

            // Decide: swallow vs rethrow.
            // Rethrow if you want the message to be retried / moved to DLQ.
            throw;
        }
    }
}