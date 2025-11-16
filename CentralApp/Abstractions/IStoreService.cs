using CentralApp.Database.Models;
using Shared.Abstractions;

namespace CentralApp.Abstractions;

/// <summary>
/// Database service for central stores.
/// </summary>
public interface IStoreService : IDbService<CentralStore>
{
}