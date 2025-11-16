using System.Collections.Concurrent;
using System.Linq.Expressions;
using Shared.Abstractions;
using Shared.Abstractions.Caches;
using ZiggyCreatures.Caching.Fusion;

namespace Shared.CommonLogic;

public class CacheService(
    IFusionCache _fusionCache,
    ICacheDataSourceFactory _dataSourceFactory)
    : ICacheService
{
    private readonly ConcurrentDictionary<string, byte> _trackedFusionKeys = [];

    public async Task<IReadOnlyDictionary<TKey, TDto>> GetAllAsync<TDto, TKey>(
        CancellationToken cancellationToken = default) where TKey : notnull
    {
        ICacheDataSource<TDto, TKey> dataSource =
            _dataSourceFactory.GetDataSource<TDto, TKey>();

        string fusionKey = BuildFusionKey<TDto>(dataSource.Category);
        _trackedFusionKeys.TryAdd(fusionKey, 0);

        FusionCacheEntryOptions entryOptions = new()
        {
            Duration = TimeSpan.FromHours(1),
            FailSafeMaxDuration = TimeSpan.FromMinutes(30),
            IsFailSafeEnabled = true
        };

        Dictionary<TKey, TDto> dict = await _fusionCache.GetOrSetAsync(
            fusionKey,
            async ct => await BuildDictionaryAsync(dataSource, ct),
            entryOptions,
            cancellationToken);

        return dict;
    }

    public async Task<TDto?> GetByKeyAsync<TDto, TKey>(
        TKey key,
        CancellationToken cancellationToken = default) where TKey : notnull
    {
        IReadOnlyDictionary<TKey, TDto> dict =
            await GetAllAsync<TDto, TKey>(cancellationToken);

        return dict.GetValueOrDefault(key);
    }

    public async Task RestartAsync(CancellationToken cancellationToken = default)
    {
        foreach (string fusionKey in _trackedFusionKeys.Keys)
            await _fusionCache.RemoveAsync(fusionKey, null, cancellationToken);

        _trackedFusionKeys.Clear();
    }

    private static async Task<Dictionary<TKey, TDto>> BuildDictionaryAsync<TDto, TKey>(
        ICacheDataSource<TDto, TKey> dataSource,
        CancellationToken cancellationToken) where TKey : notnull
    {
        IReadOnlyCollection<TDto> allItems =
            await dataSource.LoadAllAsync(cancellationToken);

        Expression<Func<TDto, TKey>> expr = dataSource.KeySelector;
        Func<TDto, TKey> keySelector = expr.Compile();

        Dictionary<TKey, TDto> dict = [];

        foreach (TDto item in allItems)
        {
            TKey key = keySelector(item);
            dict[key] = item;
        }

        return dict;
    }

    private static string BuildFusionKey<TDto>(string category)
    {
        string typeName = typeof(TDto).FullName ?? typeof(TDto).Name;
        return $"global:{category}:{typeName}";
    }
}