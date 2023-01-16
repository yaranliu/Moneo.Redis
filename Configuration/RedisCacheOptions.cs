using Newtonsoft.Json;

namespace Moneo.Redis.Configuration;

/// <summary>
/// Configuration options for Cache Class which will be derived from RedisCache 
/// </summary>
public class RedisCacheOptions
{
    /// <summary>
    /// Domain name for the specific cache. If specified, all cache items will be prefixed with this value.
    /// <example>Domain = User Cache</example>
    /// </summary>
    public string? Domain { get; set; }
    /// <summary>
    /// Settings for NewtonSoft JsonSerializer, that is being used in object serialization and deserialization
    /// </summary>
    public JsonSerializerSettings JsonSerializerSettings { get; set; }
    /// <summary>
    /// Database number of the Redis where the target cache will be created.
    /// </summary>
    public int Database { get; set; }
    /// <summary>
    /// Default expiration time in milliseconds for the items to be created. 
    /// </summary>
    public int Expiration { get; set; }
    
    public RedisCacheOptions()
    {
        Database = DefaultConfiguration.Database;
        Expiration = DefaultConfiguration.Expiration;
        JsonSerializerSettings = DefaultConfiguration.JsonSerializerSettings;
    }
    
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}