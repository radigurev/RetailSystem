namespace StoreApp.Helpers;

/// <summary>
/// Constants for Store project
/// </summary>
public class StoreConstants
{
    /// <summary>
    /// Key for config table getting store guid
    /// </summary>
    public static readonly string STORE_ID_KEY = "StoreId";
    
    public static readonly string CENTRAL_EXCHANGE_ROUTING_KEY = "StoreRoutingKey";
    
    public static readonly string QUEUE_NAME_KEY = "StoreQueueKey";
    
    public static readonly string CENTRAL_SYNC_KEY = "CentralSyncKey";
}