using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CentralApp.CommonLogic.DatabaseServices;
using CentralApp.Database;
using CentralApp.Database.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Assert = Xunit.Assert;

namespace RetailSystem.Tests.CentralApp;

public class CentralProductServiceTests
{
    private static CentralDbContext CreateCentralDbContext()
    {
        DbContextOptions<CentralDbContext> options =
            new DbContextOptionsBuilder<CentralDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        return new CentralDbContext(options);
    }

    [Fact]
    public async Task UpsertProduct_CreatesWhenNotFound()
    {
        await using CentralDbContext context = CreateCentralDbContext();
        ProductService service = new(context);

        Guid storeId = Guid.NewGuid();

        CentralProduct newProduct = new()
        {
            Name = "Central Product",
            Description = "Central desc",
            Price = 15.0m,
            MinPrice = 10.0m,
            SourceStoreId = storeId
        };

        Expression<Func<CentralProduct, bool>> predicate =
            p => p.Name == "Central Product" && p.SourceStoreId == storeId;

        CentralProduct result = await service.UpsertProduct(predicate, newProduct, CancellationToken.None);

        CentralProduct dbProduct = context.Products.Single();
        Assert.Equal("Central Product", dbProduct.Name);
        Assert.Equal(storeId, dbProduct.SourceStoreId);
        Assert.Equal(dbProduct.Id, result.Id);
    }
}