using Microsoft.Extensions.DependencyInjection;
using Shared.Abstractions;
using Shared.Abstractions.Caches;

namespace Shared.CommonLogic;

/// <summary>
/// Service that returns all services implementing ICacheDataSource
/// </summary>
/// <param name="_serviceProvider"></param>
public class CacheDataSourceFactory(IServiceProvider _serviceProvider) : ICacheDataSourceFactory
{
    public ICacheDataSource<TDto, TKey> GetDataSource<TDto, TKey>()
        => _serviceProvider.GetRequiredService<ICacheDataSource<TDto, TKey>>();
}