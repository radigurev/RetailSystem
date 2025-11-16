using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace Shared.Messaging;

/// <summary>
/// Single class that:
/// - Manages RabbitMQ connection & channel
/// - Runs as a BackgroundService (IHostedService)
/// - Exposes IRabbitMqService for other services to use.
/// </summary>
public class RabbitMqService : IMqService
{
    private readonly ConnectionFactory _factory;
    private IConnection? _connection;
    private IChannel? _channel;

    private readonly TaskCompletionSource<bool> _readyTcs =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public RabbitMqService(IConfiguration configuration)
    {
        string host = configuration["RabbitMQ:HostName"] ?? "localhost";
        string user = configuration["RabbitMQ:UserName"] ?? "guest";
        string pass = configuration["RabbitMQ:Password"] ?? "guest";

        _factory = new ConnectionFactory
        {
            HostName = host,
            UserName = user,
            Password = pass
        };
    }

    public IChannel Channel =>
        _channel ?? throw new InvalidOperationException("RabbitMQ channel not initialized yet.");

    /// <inheritdoc/>
    public async Task InitializeAsync(
        IReadOnlyList<ExchangeDeclareDTO> exchanges,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _connection = await _factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

            foreach (ExchangeDeclareDTO exchange in exchanges)
                await _channel.ExchangeDeclareAsync(
                    exchange: exchange.Exchange,
                    type: exchange.ExchangeType,
                    durable: exchange.Durable,
                    autoDelete: exchange.AutoDelete,
                    arguments: null,
                    cancellationToken: cancellationToken
                );

            _readyTcs.TrySetResult(true);
        }
        catch (Exception ex)
        {
            _readyTcs.TrySetException(ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public Task WaitUntilReadyAsync(CancellationToken cancellationToken = default)
        => _readyTcs.Task.WaitAsync(cancellationToken);

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
        {
            try
            {
                await _channel.CloseAsync();
            }
            finally
            {
                await _channel.DisposeAsync();
                _channel = null;
            }
        }

        if (_connection != null)
        {
            try
            {
                await _connection.CloseAsync();
            }
            finally
            {
                await _connection.DisposeAsync();
                _connection = null;
            }
        }
    }
}