namespace Shared.Abstractions.Caches;

public interface ICacheDataSourceFactory
{
    ICacheDataSource<TDto, TKey> GetDataSource<TDto, TKey>();
}