using Shared.Abstractions;
using StoreApp.Database.Models;

namespace StoreApp.Abstractions;

/// <summary>
/// Database service for store configuration entries.
/// </summary>
public interface IConfigService : IDbService<Config>
{
    
}