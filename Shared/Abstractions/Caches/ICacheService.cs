namespace Shared.Abstractions.Caches;

public interface ICacheService
{
    /// <summary>
    /// Gets a single DTO by key, using the cache for the given data source.
    /// </summary>
    Task<TDto?> GetByKeyAsync<TDto, TKey>(
        TKey key,
        CancellationToken cancellationToken = default) where TKey : notnull;

    /// <summary>
    /// Gets the full cached dictionary for the given data source.
    /// </summary>
    Task<IReadOnlyDictionary<TKey, TDto>> GetAllAsync<TDto, TKey>(
        CancellationToken cancellationToken = default) where TKey : notnull;

    /// <summary>
    /// Clears all cache entries managed by this global cache service.
    /// </summary>
    Task RestartAsync(CancellationToken cancellationToken = default);
}