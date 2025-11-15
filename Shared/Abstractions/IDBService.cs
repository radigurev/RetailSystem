namespace Shared.Abstractions;

using System.Linq.Expressions;

/// <summary>
/// Generic database service for basic CRUD operations.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IDbService<TEntity>
    where TEntity : BaseEntity
{
    /// <summary>
    /// Gets a single entity matching the predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets all entities, optionally filtered by predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    Task<IReadOnlyList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? predicate,
        CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new entity.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    Task<TEntity> CreateAsync(
        TEntity entity,
        CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    Task<TEntity?> UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken);

    /// <summary>
    /// Deletes entities matching the predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    Task<bool> DeleteAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken);
}

