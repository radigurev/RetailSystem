using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreApp.CommonLogic;
using StoreApp.Database;
using StoreApp.Database.Models;
using Xunit;
using Assert = Xunit.Assert;

namespace RetailSystem.Tests.StoreApp;

public class StoreProductServiceTests
{
    private static StoreDbContext CreateStoreDbContext()
    {
        DbContextOptions<StoreDbContext> options =
            new DbContextOptionsBuilder<StoreDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        return new StoreDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_PersistsProductAndSetsTimestamps()
    {
        using StoreDbContext context = CreateStoreDbContext();
        ProductService service = new ProductService(context);

        Product product = new Product
        {
            Name = "Test Product",
            Description = "Test description",
            Price = 10.0m,
            MinPrice = 5.0m
        };

        Product created = await service.CreateAsync(product, CancellationToken.None);

        Product? fromDb = await context.Products.FirstOrDefaultAsync();
        Assert.NotNull(fromDb);

        Assert.Equal("Test Product", fromDb!.Name);
        Assert.NotEqual(default, fromDb.CreatedAt);
        Assert.NotEqual(default, fromDb.UpdatedAt);
    }

    [Fact]
    public async Task UpsertProduct_UpdatesExistingProductWhenFound()
    {
        using StoreDbContext context = CreateStoreDbContext();
        ProductService service = new ProductService(context);

        Product existing = new Product
        {
            Name = "Old Name",
            Description = "Old",
            Price = 1.0m,
            MinPrice = 0.5m
        };

        context.Products.Add(existing);
        await context.SaveChangesAsync();

        Product updateEntity = new Product
        {
            Id = existing.Id,
            Name = "New Name",
            Description = "New",
            Price = 2.0m,
            MinPrice = 1.0m
        };

        Expression<Func<Product, bool>> predicate = p => p.Id == existing.Id;

        Product result = await service.UpsertProduct(predicate, updateEntity, CancellationToken.None);

        Product dbProduct = context.Products.Single();
        Assert.Equal(existing.Id, dbProduct.Id);
        Assert.Equal("New Name", dbProduct.Name);
        Assert.Equal(2.0m, dbProduct.Price);
        Assert.Equal(1.0m, dbProduct.MinPrice);
        Assert.Equal(dbProduct.Id, result.Id);
        Assert.Equal("New Name", result.Name);
    }
}
