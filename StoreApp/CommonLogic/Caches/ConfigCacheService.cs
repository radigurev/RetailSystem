using System.Linq.Expressions;
using AutoMapper;
using Shared.Abstractions;
using Shared.Abstractions.Caches;
using StoreApp.Abstractions;
using StoreApp.Database.Models;
using StoreApp.Models;

namespace StoreApp.CommonLogic.Caches;

/// <summary>
/// Implementation of config caching
/// </summary>
/// <param name="_configService"></param>
/// <param name="_mapper"></param>
internal class ConfigCacheService(
    IConfigService _configService,
    IMapper _mapper) : ICacheDataSource<ConfigDTO, string>
{
    /// <inheritdoc/>
    public string Category => nameof(Config);
    
    /// <inheritdoc/>
    public Expression<Func<ConfigDTO, string>> KeySelector => c => c.Key;

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<ConfigDTO>> LoadAllAsync(CancellationToken cancellationToken)
    {
        IEnumerable<Config> configs = await _configService.GetAllAsync(null, cancellationToken);
        
        return [.. configs.Select(_mapper.Map<ConfigDTO>)];
    }
}