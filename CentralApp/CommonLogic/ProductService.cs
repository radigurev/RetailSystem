using System.Linq.Expressions;
using CentralApp.Abstractions;
using CentralApp.Database;
using CentralApp.Database.Models;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using Shared.Exceptions;

namespace CentralApp.CommonLogic;

/// <summary>
/// EF-based implementation of the central product database service.
/// </summary>
public class ProductService(CentralDbContext _dbContext) : IProductService
{
    /// <summary>
    /// Gets a single central product matching the predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    public async Task<CentralProduct> GetAsync(
        Expression<Func<CentralProduct, bool>> predicate,
        CancellationToken cancellationToken)
    {
        CentralProduct? entity = await _dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(predicate, cancellationToken);

        return entity ?? throw new EntityNotFoundException("Product not found");
    }

    /// <summary>
    /// Gets all central products, optionally filtered by predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    public async Task<IEnumerable<CentralProduct>> GetAllAsync(
        Expression<Func<CentralProduct, bool>>? predicate,
        CancellationToken cancellationToken)
    {
        IQueryable<CentralProduct> query = _dbContext.Products.AsNoTracking();

        if (predicate != null)
            query = query.Where(predicate);

        List<CentralProduct> items = await query.ToListAsync(cancellationToken);
        return items;
    }

    /// <summary>
    /// Creates a new central product.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    public async Task<CentralProduct> CreateAsync(
        CentralProduct entity,
        CancellationToken cancellationToken)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        await _dbContext.Products.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    /// <summary>
    /// Updates an existing central product.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    public async Task<CentralProduct> UpdateAsync(
        CentralProduct entity,
        CancellationToken cancellationToken)
    {
        CentralProduct? existing = await _dbContext.Products
            .FirstOrDefaultAsync(x => x.Id == entity.Id,
                cancellationToken);

        if (existing is null)
            throw new EntityNotFoundException("Product not found");

        entity.UpdatedAt = DateTime.UtcNow;

        _dbContext.Entry(existing).CurrentValues.SetValues(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return existing;
    }

    /// <summary>
    /// Deletes central products matching the predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    public async Task DeleteAsync(
        Expression<Func<CentralProduct, bool>> predicate,
        CancellationToken cancellationToken)
    {
        CentralProduct? entity = await _dbContext.Products
            .Where(predicate)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity is null)
            throw new EntityNotFoundException("Product not found");

        _dbContext.Products.RemoveRange(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<CentralProduct> UpsertProduct(
        Expression<Func<CentralProduct, bool>> productPredicate,
        CentralProduct productEntity,
        CancellationToken cancellationToken)
    {
        CentralProduct? product = await _dbContext.Products.FirstOrDefaultAsync(productPredicate, cancellationToken);

        if (product is null)
            return await CreateAsync(productEntity, cancellationToken);
        
        return await UpdateAsync(productEntity, cancellationToken);
    }
}