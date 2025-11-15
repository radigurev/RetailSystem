using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using StoreApp.Abstractions;
using StoreApp.Database;
using StoreApp.Database.Models;

namespace StoreApp.CommonLogic;

/// <summary>
/// EF-based implementation of the store configuration database service.
/// </summary>
public class ConfigService(StoreDbContext dbContext) : IConfigService
{
    private readonly StoreDbContext _dbContext = dbContext;

    /// <summary>
    /// Gets a single configuration entry matching the predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    public async Task<Config> GetAsync(
        Expression<Func<Config, bool>> predicate,
        CancellationToken cancellationToken)
    {
        Config? entity = await _dbContext.ConfigEntries
            .AsNoTracking()
            .FirstOrDefaultAsync(predicate, cancellationToken);

        return entity ?? throw new KeyNotFoundException("Config not found");
    }

    /// <summary>
    /// Gets all configuration entries, optionally filtered by predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    public async Task<IEnumerable<Config>> GetAllAsync(
        Expression<Func<Config, bool>>? predicate,
        CancellationToken cancellationToken)
    {
        IQueryable<Config> query = _dbContext.ConfigEntries.AsNoTracking();

        if (predicate != null)
            query = query.Where(predicate);

        List<Config> items = await query.ToListAsync(cancellationToken);
        return items;
    }

    /// <summary>
    /// Creates a new configuration entry.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    public async Task<Config> CreateAsync(
        Config entity,
        CancellationToken cancellationToken)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        await _dbContext.ConfigEntries.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    /// <summary>
    /// Updates an existing configuration entry.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    public async Task<Config> UpdateAsync(
        Config entity,
        CancellationToken cancellationToken)
    {
        Config? existing = await _dbContext.ConfigEntries
            .FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);

        if (existing == null)
            throw new KeyNotFoundException("Config not found");

        entity.CreatedAt = existing.CreatedAt;
        entity.UpdatedAt = DateTime.UtcNow;

        _dbContext.Entry(existing).CurrentValues.SetValues(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return existing;
    }

    /// <summary>
    /// Deletes configuration entries matching the predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    public async Task DeleteAsync(
        Expression<Func<Config, bool>> predicate,
        CancellationToken cancellationToken)
    {
        List<Config> entities = await _dbContext.ConfigEntries
            .Where(predicate)
            .ToListAsync(cancellationToken);

        if (entities.Count == 0)
            throw new KeyNotFoundException("Config not found");

        _dbContext.ConfigEntries.RemoveRange(entities);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}