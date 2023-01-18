using Microsoft.Extensions.DependencyInjection;
using Moneo.Redis.Attributes;
using Moneo.Redis.Configuration;
using Moneo.Redis.Exceptions;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Moneo.Redis;

/// <summary>
/// Base Class for creating virtual caches using the same Redis configuration
/// <example>
/// class UserCache : RedisCache
/// class JwtCache : RedisCache
/// class PhotoCache : RedisCache
/// </example>
/// </summary>
public class RedisCache : IRedisCache
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IDatabase Database { get; }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public JsonSerializerSettings JsonSerializerSettings { get; }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string? Domain { get; }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Expiration { get; }    
    protected RedisCache(IServiceProvider provider) 
    {
        var connectionMultiplexer = provider.GetService<IConnectionMultiplexer>();
        if (connectionMultiplexer == null) throw new RedisCacheIsNotAvailableException();
        Database = connectionMultiplexer.GetDatabase(DefaultConfiguration.Database);
        if (Database == null) throw new RedisCacheIsNotAvailableException();
        JsonSerializerSettings = DefaultConfiguration.JsonSerializerSettings;
        Expiration = DefaultConfiguration.Expiration;
    }
    protected RedisCache(IServiceProvider provider, RedisCacheOptions options) 
        : this(provider)
    {
        var connectionMultiplexer = provider.GetService<IConnectionMultiplexer>();
        if (connectionMultiplexer == null) throw new RedisCacheIsNotAvailableException();
        Database = connectionMultiplexer.GetDatabase(options.Database);
        if (Database == null) throw new RedisCacheIsNotAvailableException();
        JsonSerializerSettings = options.JsonSerializerSettings;
        Domain = options.Domain;
        Expiration = options.Expiration;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="id"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public string ItemKey<T>(string id) => $"{KeyPrefix<T>()}{DefaultConfiguration.Separator}{id}";
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public string ItemKey(object obj) => $"{KeyPrefix(obj)}{DefaultConfiguration.Separator}{ObtainKeyFromObject(obj)}";
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public KeyValuePair<RedisKey, RedisValue> GetKeyValuePair(object obj)
    {
        var s = JsonConvert.SerializeObject(obj, JsonSerializerSettings);
        var v = new RedisValue(s);
        var k = new RedisKey(ItemKey(obj));
        return new KeyValuePair<RedisKey, RedisValue>(k, v);
    }
    /// <summary>
    /// Internal method to resolve Collection name from the object to be used in Cache Key
    /// </summary>
    /// <param name="obj">Object to check</param>
    /// <returns>StoredAs parameter of CanBeCached attribute or object's type name if not specified</returns>
    private static string Collection(object obj) => AttributeResolver.GetCollectionName(obj);
    /// <summary>
    /// Internal method to resolve Collection name for the class be used in Cache Key
    /// </summary>
    /// <typeparam name="T">Class or struct to check</typeparam>
    /// <returns>StoredAs parameter of CanBeCached attribute or Class name (T) if not specified</returns>
    private static string Collection<T>() => AttributeResolver.GetCollectionName<T>();
    /// <summary>
    /// Internal method to calculate the key of the item. Uses CacheKey attribute
    /// </summary>
    /// <param name="obj">Object to check</param>
    /// <returns>Key prefix for the object</returns>
    private string KeyPrefix(object obj) => string.IsNullOrWhiteSpace(Domain) ? $"{Collection(obj)}" : $"{Domain}{DefaultConfiguration.Separator}{Collection(obj)}";
    /// <summary>
    /// Internal method to calculate the key of the item. Uses CacheKey attribute
    /// </summary>
    /// <typeparam name="T">Class or struct to check</typeparam>
    /// <returns>Key prefix for T</returns>
    private string KeyPrefix<T>() => string.IsNullOrWhiteSpace(Domain) ? $"{Collection<T>()}" : $"{Domain}{DefaultConfiguration.Separator}{Collection<T>()}";
    /// <summary>
    /// Internal method to calculate Item key
    /// </summary>
    /// <param name="obj">Object to check</param>
    /// <returns>String to be used as key of the cache item</returns>
    private static string ObtainKeyFromObject(object obj) => AttributeResolver.GetKeyValue(obj);

}