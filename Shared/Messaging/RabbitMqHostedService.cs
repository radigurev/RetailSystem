using Microsoft.Extensions.Hosting;

namespace Shared.Messaging;

public class RabbitMqHostedService(IMqService mqService) : IHostedService
{
    private readonly IMqService _mqService = mqService;
    

    /// <summary>
    /// Initializes the MQ channel
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async Task StartAsync(CancellationToken cancellationToken)
        => await _mqService.InitializeAsync(cancellationToken);


    /// <summary>
    /// Disposes MQ
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task StopAsync(CancellationToken cancellationToken)
        => await _mqService.DisposeAsync();
}