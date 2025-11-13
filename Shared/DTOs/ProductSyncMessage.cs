using Shared.Enums;

namespace Shared.DTOs;

/// <summary>
/// 
/// </summary>
public record ProductSyncMessage(
    MQMessageType Type,
    Guid StoreId,
    ProductDTO Product);