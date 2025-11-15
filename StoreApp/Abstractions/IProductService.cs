using Shared.Abstractions;
using StoreApp.Database.Models;

namespace StoreApp.Abstractions;

public interface IProductService : IDbService<Product>
{
    
}