using System.Linq.Expressions;
using AutoMapper;
using CentralApp.Abstractions;
using CentralApp.Database.Models;
using Shared.Abstractions;
using Shared.DTOs;

namespace CentralApp.CommonLogic.Producers;

/// <summary>
/// Handles what happens with products coming from MQ
/// </summary>
internal class ProductSyncHandler: IProductSyncHandler
{
    private readonly ILogger<ProductSyncHandler> _logger;
    private readonly IStoreService _storeService;
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public ProductSyncHandler(
        ILogger<ProductSyncHandler> logger,
        IStoreService storeService,
        IProductService productService,
        IMapper mapper)
    {
        _logger = logger;
        _storeService = storeService;
        _productService = productService;
        _mapper = mapper;
    }
    /// <inheritdoc/>
    public async Task HandleAsync(ProductSyncMessage message, CancellationToken cancellationToken)
    {
        try
        {
            Expression<Func<CentralStore, bool>> storePredicate =
                x => x.Id == message.StoreGuid;

            CentralStore store =
                await _storeService.GetAsync(storePredicate, cancellationToken);

            Expression<Func<CentralProduct, bool>> productPredicate =
                x => x.SourceStoreId == store.Id;

            CentralProduct toCreate = _mapper.Map<CentralProduct>(message.Product);
            toCreate.SourceStoreId = message.StoreGuid;

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