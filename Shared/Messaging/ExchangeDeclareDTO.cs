namespace Shared.Messaging;

/// <summary>
/// DTO to transfer what exchanges are going to be used
/// </summary>
/// <param name="Exchange"></param>
/// <param name="ExchangeType"></param>
/// <param name="Durable"></param>
/// <param name="AutoDelete"></param>
public record ExchangeDeclareDTO(
    string Exchange,
    string ExchangeType,
    bool Durable,
    bool AutoDelete
    );