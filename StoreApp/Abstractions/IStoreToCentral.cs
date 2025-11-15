using Shared.DTOs;

namespace StoreApp.Abstractions;

/// <summary>
/// Communication with central Message Broker
/// </summary>
public interface IStoreToCentral
{
    /// <summary>
    /// Publishes changes or creation of a product
    /// </summary>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task PublishAsync(
        ProductSyncMessage message,
        CancellationToken cancellationToken = default);
}