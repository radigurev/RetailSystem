using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Shared.Abstractions.Caches;
using Shared.CommonLogic;
using Xunit;
using ZiggyCreatures.Caching.Fusion;
using Assert = Xunit.Assert;

namespace RetailSystem.Tests.Shared;

public class CacheServiceTests
{
    private record TestDto(int Id, string Name);

    private class TestCacheDataSource : ICacheDataSource<TestDto, int>
    {
        private readonly IReadOnlyCollection<TestDto> _items;

        public TestCacheDataSource(IReadOnlyCollection<TestDto> items)
        {
            _items = items;
        }

        public string Category => "TestCategory";

        public Expression<Func<TestDto, int>> KeySelector => dto => dto.Id;

        public Task<IReadOnlyCollection<TestDto>> LoadAllAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_items);
        }
    }

    private class TestCacheDataSourceFactory : ICacheDataSourceFactory
    {
        private readonly ICacheDataSource<TestDto, int> _testDataSource;

        public TestCacheDataSourceFactory(ICacheDataSource<TestDto, int> testDataSource)
        {
            _testDataSource = testDataSource;
        }

        public ICacheDataSource<TDto, TKey> GetDataSource<TDto, TKey>()
        {
            if (typeof(TDto) == typeof(TestDto) && typeof(TKey) == typeof(int))
            {
                return (ICacheDataSource<TDto, TKey>)_testDataSource;
            }

            throw new InvalidOperationException("Unexpected DTO/key type requested from factory.");
        }
    }

    [Fact]
    public async Task GetAllAsync_ReturnsDictionaryFromDataSource()
    {
        List<TestDto> items =
        [
            new TestDto(1, "First"),
            new TestDto(2, "Second")
        ];

        TestCacheDataSource dataSource = new(items);
        TestCacheDataSourceFactory factory = new(dataSource);

        IFusionCache fusionCache = new FusionCache(new FusionCacheOptions());

        CacheService service = new(fusionCache, factory);

        IReadOnlyDictionary<int, TestDto> result =
            await service.GetAllAsync<TestDto, int>();

        Assert.Equal(2, result.Count);
        Assert.Equal("First", result[1].Name);
        Assert.Equal("Second", result[2].Name);
    }

    [Fact]
    public async Task GetByKeyAsync_ReturnsSingleItemWhenKeyExists()
    {
        List<TestDto> items =
        [
            new TestDto(1, "First"),
            new TestDto(2, "Second")
        ];

        TestCacheDataSource dataSource = new(items);
        TestCacheDataSourceFactory factory = new(dataSource);

        IFusionCache fusionCache = new FusionCache(new FusionCacheOptions());

        CacheService service = new(fusionCache, factory);

        TestDto? result =
            await service.GetByKeyAsync<TestDto, int>(2);

        Assert.NotNull(result);
        Assert.Equal(2, result!.Id);
        Assert.Equal("Second", result.Name);
    }
}
