using System.Linq.Expressions;

namespace Shared.Abstractions.Caches;

public interface ICacheDataSource<TDto, TKey>
{
    /// <summary>
    /// Logical category of the cache
    /// (e.g. "config", "product", "user"). Used in cache key.
    /// </summary>
    string Category { get; }

    /// <summary>
    /// Expression that selects the key from the DTO.
    /// Example: dto => dto.Key or dto => dto.Id.
    /// </summary>
    Expression<Func<TDto, TKey>> KeySelector { get; }

    /// <summary>
    /// Loads and maps all DTOs for this category.
    /// Implementation can query EF, map with AutoMapper, etc.
    /// </summary>
    Task<IReadOnlyCollection<TDto>> LoadAllAsync(CancellationToken cancellationToken);
}