using System.Linq.Expressions;
using CentralApp.Database.Models;
using Shared.Abstractions;
using Shared.DTOs;

namespace CentralApp.Abstractions;

/// <summary>
/// Database service for central products.
/// </summary>
public interface IProductService : IDbService<CentralProduct>
{
    Task<CentralProduct> UpsertProduct(Expression<Func<CentralProduct, bool>> productPredicate, CentralProduct messageProduct, CancellationToken cancellationToken);
}