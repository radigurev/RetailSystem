using CentralApp.Database.Models;
using Shared.Abstractions;

namespace CentralApp.Abstractions;

/// <summary>
/// Database service for central products.
/// </summary>
public interface IProductService : IDbService<CentralProduct>
{
}