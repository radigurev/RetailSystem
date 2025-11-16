using Shared.Enums;

namespace Shared.DTOs;

/// <summary>
/// Entity used to send to MQ for product
/// </summary>
public record ProductSyncMessage(
    MQMessageType Type,
    Guid StoreGuid,
    ProductDTO Product);