using RabbitMQ.Client;

namespace Shared.Messaging;

/// <summary>
/// Interface for creating comunication with message broker
/// </summary>
public interface IMqService : IAsyncDisposable
{
    IChannel Channel { get; }

    /// <summary>
    /// Called by the hosted service at startup to init connection/channel.
    /// Safe to call multiple times; only first call does the work.
    /// </summary>
    internal Task InitializeAsync(
        IReadOnlyList<ExchangeDeclareDTO> exchanges,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Await this before using Channel to ensure it is ready.
    /// </summary>
    Task WaitUntilReadyAsync(CancellationToken cancellationToken = default);
}