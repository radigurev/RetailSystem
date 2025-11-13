using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Enums;
using StoreApp.Abstractions;

namespace StoreApp.Extensions;

[ApiController]
[Route("api/[controller]")]
public class SystemController(IStoreToCentral storeToCentral) : ControllerBase
{
    private readonly IStoreToCentral _storeToCentral = storeToCentral;
    
    [HttpPost]
    public async Task<IActionResult> CreateProduct(
        [FromBody] ProductDTO productDto,
        CancellationToken cancellationToken)
    {
        ProductSyncMessage message = new(
            Type: MQMessageType.Create,
            StoreId: Guid.Parse("11111111-1111-1111-1111-111111111111"), // your store ID
            Product: productDto);

        await _storeToCentral.PublishAsync(message, cancellationToken);

        return Created(nameof(CreateProduct) ,productDto);
    }
}