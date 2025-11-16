using System.Linq.Expressions;
using CentralApp.Abstractions;
using CentralApp.Database;
using CentralApp.Database.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Exceptions;

namespace CentralApp.CommonLogic.DatabaseServices;

/// <summary>
/// EF-based implementation of the central store database service.
/// </summary>
public class StoreService(CentralDbContext dbContext) : IStoreService
{
    private readonly CentralDbContext _dbContext = dbContext;

    /// <summary>
    /// Gets a single store matching the predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    public async Task<CentralStore> GetAsync(
        Expression<Func<CentralStore, bool>> predicate,
        CancellationToken cancellationToken)
    {
        CentralStore? entity = await _dbContext.Stores
            .AsNoTracking()
            .FirstOrDefaultAsync(predicate, cancellationToken);

        return entity ?? throw new EntityNotFoundException("Store Not Found");
    }

    /// <summary>
    /// Gets all stores, optionally filtered by predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    public async Task<IEnumerable<CentralStore>> GetAllAsync(
        Expression<Func<CentralStore, bool>>? predicate,
        CancellationToken cancellationToken)
    {
        IQueryable<CentralStore> query = _dbContext.Stores.AsNoTracking();

        if (predicate != null)
            query = query.Where(predicate);

        List<CentralStore> items = await query.ToListAsync(cancellationToken);
        return items;
    }

    /// <summary>
    /// Creates a new store.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    public async Task<CentralStore> CreateAsync(
        CentralStore entity,
        CancellationToken cancellationToken)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        await _dbContext.Stores.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    /// <summary>
    /// Updates an existing store.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    public async Task<CentralStore> UpdateAsync(
        CentralStore entity,
        CancellationToken cancellationToken)
    {
        CentralStore? existing = await _dbContext.Stores
            .FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);

        if (existing == null)
            throw new EntityNotFoundException("Store Not Found");

        entity.UpdatedAt = DateTime.UtcNow;

        _dbContext.Entry(existing).CurrentValues.SetValues(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return existing;
    }

    /// <summary>
    /// Deletes stores matching the predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    public async Task DeleteAsync(
        Expression<Func<CentralStore, bool>> predicate,
        CancellationToken cancellationToken)
    {
        CentralStore? entities = await _dbContext.Stores
            .Where(predicate)
            .FirstOrDefaultAsync(cancellationToken);

        if (entities is null)
            throw new EntityNotFoundException("Store Not Found");
        _dbContext.Stores.RemoveRange(entities);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}