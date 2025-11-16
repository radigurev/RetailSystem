using Microsoft.Extensions.Hosting;

namespace Shared.Messaging;

/// <summary>
/// Hosted service to initialize MQ 
/// </summary>
/// <param name="mqService"></param>
public class RabbitMqHostedService(
    IMqService mqService,
    IReadOnlyList<ExchangeDeclareDTO> exchanges) : IHostedService
{
    private readonly IReadOnlyList<ExchangeDeclareDTO> _exchanges = exchanges;
    private readonly IMqService _mqService = mqService;
    
    /// <summary>
    /// Initializes the MQ channel
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async Task StartAsync(CancellationToken cancellationToken)
        => await _mqService.InitializeAsync(_exchanges, cancellationToken);
    
    /// <summary>
    /// Disposes MQ
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task StopAsync(CancellationToken cancellationToken)
        => await _mqService.DisposeAsync();
}