using System.Linq.Expressions;
using AutoMapper;
using CentralApp.Abstractions;
using CentralApp.Database.Models;
using CentralApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Abstractions.Caches;
using Shared.DTOs;
using Shared.Enums;
using Shared.Models;
using StoreApp.Models;
using static CentralApp.Helpers.CentralConstants;

namespace CentralApp.Controllers;

/// <summary>
/// Central products API endpoints.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController(
    IProductService _productService,
    IStoreService _storeService,
    IMapper _mapper,
    ILogger<ProductsController> _logger,
    ICacheService _cacheService,
    ICentralToStores _centralToStores) : ControllerBase
{

    /// <summary>
    /// Gets a product by id from the central database.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDTO>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            Expression<Func<CentralProduct, bool>> predicate = x => x.Id == id;

            CentralProduct? entity =
                await _productService.GetAsync(predicate, cancellationToken);

            ProductDTO dto = _mapper.Map<ProductDTO>(entity);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting product by id {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error.");
        }
    }

    /// <summary>
    /// Gets all products from the central database.
    /// </summary>
    /// <param name="cancellationToken"></param>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDTO>>> GetAll(
        CancellationToken cancellationToken)
    {
        try
        {
            IReadOnlyList<CentralProduct> entities =
                [.. await _productService.GetAllAsync(null, cancellationToken)];

            List<ProductDTO> result = _mapper.Map<List<ProductDTO>>(entities);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting all products");
            return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error.");
        }
    }

    /// <summary>
    /// Creates a new product in the central database for a source store.
    /// </summary>
    /// <param name="sourceStoreId"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    [HttpPost("{sourceStoreId:guid}")]
    public async Task<ActionResult<ProductDTO>> Create(
        Guid sourceStoreId,
        [FromBody] ProductCreateRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            Expression<Func<CentralStore, bool>> storePredicate =
                x => x.Id == sourceStoreId;

            CentralStore store =
                await _storeService.GetAsync(storePredicate, cancellationToken);

            CentralProduct entity = _mapper.Map<CentralProduct>(request);
            entity.SourceStoreId = store.Id;

            CentralProduct created =
                await _productService.CreateAsync(entity, cancellationToken);

            ProductDTO dto = _mapper.Map<ProductDTO>(created);

            await AlertCentral(dto, MQMessageType.Create, sourceStoreId, cancellationToken);
            
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating product for store {StoreId}", sourceStoreId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error.");
        }
    }

    /// <summary>
    /// Updates an existing product in the central database.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductDTO>> Update(
        Guid id,
        [FromBody] ProductUpdateRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            Expression<Func<CentralProduct, bool>> predicate = x => x.Id == id;

            CentralProduct? existing =
                await _productService.GetAsync(predicate, cancellationToken);

            _mapper.Map(request, existing);

            CentralProduct? updated =
                await _productService.UpdateAsync(existing, cancellationToken);

            ProductDTO dto = _mapper.Map<ProductDTO>(updated);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while updating product {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error.");
        }
    }

    /// <summary>
    /// Deletes a product from the central database.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            Expression<Func<CentralProduct, bool>> predicate = x => x.Id == id;

            await _productService.DeleteAsync(predicate, cancellationToken);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting product {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error.");
        }
    }
    
    private async Task AlertCentral(
        ProductDTO productDto,
        MQMessageType type,
        Guid storeId,
        CancellationToken cancellationToken)
    {
        
        ProductSyncMessage message = new(
            Type: type,
            StoreGuid: storeId,
            Product: productDto);

        await _centralToStores.PublishAsync(message, cancellationToken);
    }
}