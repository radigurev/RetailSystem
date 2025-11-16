using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Enums;
using StoreApp.Abstractions;

namespace StoreApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemController(IStoreToCentral storeToCentral) : ControllerBase
{
    private readonly IStoreToCentral _storeToCentral = storeToCentral;
    
    /// <summary>
    /// Create products. Its send to Central DB
    /// </summary>
    /// <param name="productDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateProduct(
        [FromBody] ProductDTO productDto,
        CancellationToken cancellationToken)
    {
   
        return Created(nameof(CreateProduct) ,productDto);
    }
}