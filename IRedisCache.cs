using Newtonsoft.Json;
using StackExchange.Redis;

namespace Moneo.Redis;

public interface IRedisCache
{
   /// <summary>
   /// Method to extract Redis Key and Value using CanBeCached and CacheItem attributes for the item to be cached.
   /// </summary>
   /// <param name="obj">Object to evaluate</param>
   /// <returns>Redis Key and Value pair</returns>
   KeyValuePair<RedisKey, RedisValue> GetKeyValuePair(object obj);
   /// <summary>
   /// Method to calculate the key for the class T with id. Uses CanBeCached attribute 
   /// </summary>
   /// <param name="id">Item identifier</param>
   /// <typeparam name="T">Class to calculate as Item</typeparam>
   /// <returns>Domain:Collection:Id</returns>
   string ItemKey<T>(string id);
   /// <summary>
   /// Method to calculate the key for the object. Uses CanBeCached and CacheKey attributes.
   /// </summary>
   /// <param name="obj">Object to calculate as Item</param>
   /// <returns></returns>
   string ItemKey(object obj);
   /// <summary>
   /// Default item expiration time in milliseconds 
   /// </summary>
   int Expiration { get; }
   /// <summary>
   /// Default Domain (prefix) for the cache 
   /// </summary>
   string? Domain { get; }
   /// <summary>
   /// Default settings for object serialization and deserialization used during cache storage and retriaval 
   /// </summary>
   JsonSerializerSettings? JsonSerializerSettings { get; }
   /// <summary>
   /// Redis Database to be used in Custom Cache methods like storing and retrieving data to/from the cache
   /// <example>SaveUserToCache(User user) {
   /// kvp = GetKeyValuePair(user);
   /// await Database.StringSetAsync(kvp.Key, kvp.Item);
   /// }</example> 
   /// </summary>
   IDatabase Database { get; }
}