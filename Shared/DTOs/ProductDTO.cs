namespace Shared.DTOs;

/// <summary>
/// DTO for Product entity
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
/// <param name="Description"></param>
/// <param name="Price"></param>
/// <param name="MinPrice"></param>
public record ProductDTO(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    decimal MinPrice
);