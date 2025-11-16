using System.Linq.Expressions;
using Shared.Abstractions;
using StoreApp.Database.Models;

namespace StoreApp.Abstractions;

public interface IProductService : IDbService<Product>
{
    public Task<Product> UpsertProduct(
        Expression<Func<Product, bool>> productPredicate,
        Product productEntity,
        CancellationToken cancellationToken);
}