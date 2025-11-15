using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Enums;
using StoreApp.Abstractions;
using StoreApp.Database.Models;
using StoreApp.Models;

using static StoreApp.Helpers.StoreConstants;

namespace StoreApp.Controllers
{
    /// <summary>
    /// Store products API endpoints.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(
        IProductService productService,
        IMapper mapper,
        IStoreToCentral storeToCentral,
        IConfigService configService) : ControllerBase
    {
        private readonly IProductService _productService = productService;
        private readonly IMapper _mapper = mapper;
        private readonly IStoreToCentral _storeToCentral = storeToCentral;
        private readonly IConfigService _configService = configService;
        
        /// <summary>
        /// Gets a product by id from the store database.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetById(
            int id,
            CancellationToken cancellationToken)
        {
            Expression<Func<Product, bool>> predicate = x => x.Id == id;
            Product entity = await _productService.GetAsync(predicate, cancellationToken);

            ProductDTO dto = _mapper.Map<ProductDTO>(entity);
            return Ok(dto);
        }

        /// <summary>
        /// Gets all products from the store database.
        /// </summary>
        /// <param name="cancellationToken"></param>
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductDTO>>> GetAll(
            CancellationToken cancellationToken)
        {
            IReadOnlyList<Product> entities =
                [.. await _productService.GetAllAsync(null, cancellationToken)];

            List<ProductDTO> result = _mapper.Map<List<ProductDTO>>(entities);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new product in the store database.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        [HttpPost]
        public async Task<ActionResult<ProductDTO>> Create(
            [FromBody] ProductCreateRequest request,
            CancellationToken cancellationToken)
        {
            Product entity = _mapper.Map<Product>(request);

            Product created =
                await _productService.CreateAsync(entity, cancellationToken);

            ProductDTO dto = _mapper.Map<ProductDTO>(created);
            await AlertCentral(dto, MQMessageType.Create, cancellationToken);
            
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        /// <summary>
        /// Updates an existing product in the store database.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProductDTO>> Update(
            int id,
            [FromBody] ProductUpdateRequest request,
            CancellationToken cancellationToken)
        {
            Expression<Func<Product, bool>> predicate = x => x.Id == id;

            Product existing =
                await _productService.GetAsync(predicate, cancellationToken);

            _mapper.Map(request, existing);

            Product updated =
                await _productService.UpdateAsync(existing, cancellationToken);

            ProductDTO dto = _mapper.Map<ProductDTO>(updated);
            await AlertCentral(dto, MQMessageType.Update, cancellationToken);
            
            return Ok(dto);
        }

        /// <summary>
        /// Deletes a product from the store database.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            int id,
            CancellationToken cancellationToken)
        {
            Expression<Func<Product, bool>> predicate = x => x.Id == id;

            await _productService.DeleteAsync(predicate, cancellationToken);
            ProductDTO dto = new(id, string.Empty, string.Empty, 0,0);

            await AlertCentral(dto, MQMessageType.Delete, cancellationToken);

            return NoContent();
        }
        
        private async Task AlertCentral(
            ProductDTO productDto,
            MQMessageType type,
            CancellationToken cancellationToken)
        {
            Expression<Func<Config, bool>> expression = c => c.Key == STORE_ID_KEY;
            Config config = await _configService.GetAsync(expression, cancellationToken);
            ProductSyncMessage message = new(
                Type: type,
                StoreId: Guid.Parse(config.Value),
                Product: productDto);

            await _storeToCentral.PublishAsync(message, cancellationToken);
        }
    }
}