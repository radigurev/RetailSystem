using System.Linq.Expressions;
using AutoMapper;
using CentralApp.Abstractions;
using CentralApp.Database.Models;
using CentralApp.Models;
using Shared.Abstractions.Caches;

namespace CentralApp.CommonLogic.Caches;

internal class ConfigCacheService(
    IConfigService _configService,
    IMapper _mapper) : ICacheDataSource<ConfigDTO, string>
{
    public string Category => nameof(Config);
    public Expression<Func<ConfigDTO, string>> KeySelector => c => c.Key;

    public async Task<IReadOnlyCollection<ConfigDTO>> LoadAllAsync(CancellationToken cancellationToken)
    {
        IEnumerable<Config> configs = await _configService.GetAllAsync(null, cancellationToken);
        
        return [.. configs.Select(_mapper.Map<ConfigDTO>)];
    }
}