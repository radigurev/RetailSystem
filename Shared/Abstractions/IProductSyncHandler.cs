using Shared.DTOs;

namespace Shared.Abstractions;

/// <summary>
/// Interface for product handling after MQ
/// </summary>
public interface IProductSyncHandler
{
    /// <summary>
    /// Handles product when received from channel
    /// </summary>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleAsync(ProductSyncMessage message, CancellationToken cancellationToken);
}