# Virtual Cache Wrapper based on Redis
## Motivation
While developing micro service architecture projects, more than one service may necessitate to access the same cache, for example ```UserProfile``` from ```UserService``` and ```OrderService```.

Moreover, there may be a need for multiple abstractions on the same Redis cache with different configurations, e.g. ```UserCache``` and ```JwtCache```.
### Dependencies
```
StackExchange.Redis
NewtonSoft.Json
Microsoft.Extensions.DependencyInjection
```
# Usage

1. Add ```StackExchange.Redis.IConnectionMultiplexer``` as a singleton to service container with ```AddRedisCache``` or in a standard manner.
2. Define object classes to be cached
3.  Create an interface for the custom cache service
4. Define a custom cache class based on the created cache interface and ```RedisCache```
5. Add custom cache service to service provider
6. Inject ```IUserCache``` to the targert service(s) via *Dependency Injection*
7.  If needed, e.g. in ```Controller (HTTP Request Handlers)```, add target service to the service container during app startup and resolve the final service via *Dependency Injection*

## Example Use

1. Add ```StackExchange.Redis.IConnectionMultiplexer``` as a singleton to service container with ```AddRedisCache``` or in a standard manner.


```c#  
collection.AddRedisCache("localhost:6379")  
  
// OR  
  
collection.AddSingleton<IConnectionMultiplexer>(x => ConnectionMultiplexer.Connect("localhost:6379"));  
```  

2. Define object classes to be cached

```c#
[CanBeCached("public-profile")]  
public class PublicUserProfile  
{  
    [CacheKey]  
    public string Id { get; set; }  
    
    public string DisplayName { get; set; }  
    
    public string AvatarUrl { get; set; }
	// ...
}
```

3. Create an interface for the custom cache service

```c#  
public interface IUserCache : IRedisCache  
{  
    Task<bool> SetUserPublicProfileAsync(PublicUserProfile profile, int ttl);  
    
    Task<PublicUserProfile> GetUserPublicProfileAsync(string id);  
    
    Task<bool> RemovePublicProfileAsync(string id);  
}  
```  

4. Define a custom cache class based on the created cache interface and ```RedisCache```

```c#  
public class UserCache : RedisCache, IUserCache  
{  
    public UserCache(IServiceProvider provider) : base(provider) {}  
    
    public UserCache(IServiceProvider provider, RedisCacheOptions options) : base(provider, options) {}  
    
    public async Task<bool> SetUserPublicProfileAsync(PublicUserProfile profile, int ttl)  
    {  
        var kvp = GetKeyValuePair(profile);  
        return await Database.StringSetAsync(kvp.Key, kvp.Value, TimeSpan.FromMilliseconds(ttl));   
    }  
      
    public async Task<PublicUserProfile> GetUserPublicProfileAsync(string id)  
    {  
        var k = new RedisKey(ItemKey<PublicUserProfile>(id));  
        var v = await Database.StringGetAsync(k);  
        if (v.IsNull) return default;  
        var profile = JsonConvert.DeserializeObject<PublicUserProfile>(v.ToString(), JsonSerializerSettings);  
        return profile;  
    }  
  
    public async Task<bool> RemovePublicProfileAsync(string id)  
    {  
        var k = new RedisKey(ItemKey<PublicUserProfile>(id));  
        return await Database.KeyDeleteAsync(k);  
    }  
}  
```  

5. Add custom cache service to service provider

```c#
collection.AddSingleton<IUserCache, UserCache>(  
    provider => new UserCache(provider, options));
```

6. Inject ```IUserCache``` to the targert service(s) via *Dependency Injection*

```c# 
public class UserService {
	
	private readonly IUserCache _userCache;
	
	public UserService(IUserCache userCache) {
		_userCache = userCache;
	}
	public Task SomeMethod(string id) {
		var profile = await _userCache.GetUserPublicProfileAsync(id);
		// ...
	}
	// ...
}
```

7. If needed, e.g. in ```Controller (HTTP Request Handlers)```, add target service to the service container during app startup and resolve the final service via *Dependency Injection*
```c#
collection.AddSingleton<UserService>();
// OR
collection.AddScoped<UserService>();
// OR
collection.AddTransient<UserService>();
```

# Attributes
## CanBeCached
Although not mandatory, ```CanBeCached``` attribute is used to define collection name for the class or struct that will be stored in Redis.

if ```CanBeCached``` attribute is missing or has no parameter, ```domain``` will be the class name, otherwise, the parameter will be used as the ```domain```

**Examples**

```c#
// Domain -> UserProfile
[CanBeCached]
public class UserProfile {
	// ...
}
```

```c#
// Domain -> user-profile
[CanBeCached("user-profile")]
public class UserProfile {
	// ...
}
```


## CacheKey
Decorate the identifier property (```string```)  of the cachable item with ```CacheKey``` attribute. If this attribute is missing or the property with this tag returns a ```null``` or empty string on run-time, ```CacheKeyIsNullOrEmptyException``` will be thrown.
```c#
[CanBeCached("user-profile")]
public class UserProfile {
	[CacheKey]
	public string Identifier {get; set; }
	public string DisplayName {get; set; }
	public string AvatarUrl { get; set;}
}
```

# Classes
## RedisCache
Base class for the custom cache to be created

### Properties (Get only)

**```IDatabase Database```**
Redis database. The custom cache will be implementing methods using this variable.

**Example:**
```c#
public async Task<bool> RemovePublicProfileAsync(string id)  
    {  
        var k = new RedisKey(ItemKey<PublicUserProfile>(id));  
        return await Database.KeyDeleteAsync(k);  
    } 
```

**```JsonSerializerSettings JsonSerializerSettings```**
Newtonsoft serializer settings which may be configured in constructor

**Example:**
```c#
public class RedisCacheOptions  
{  

	public string? Domain { get; set; }  

	public JsonSerializerSettings JsonSerializerSettings { get; set; }  

	// Redis Database number 
	public int Database { get; set; }  

	public int Expiration { get; set; }
    
    public RedisCacheOptions()  
    {
	    Database = DefaultConfiguration.Database;  
        Expiration = DefaultConfiguration.Expiration;  
        JsonSerializerSettings = DefaultConfiguration.JsonSerializerSettings;  
    }
}

public class UserCache : RedisCache, IUserCache  
{  
    public UserCache(IServiceProvider provider) : base(provider) {}  
    
    public UserCache(IServiceProvider provider, RedisCacheOptions options) : base(provider, options) {}

	// ...
}
```


**```string? Domain```**
Run-time generated **domain** for the custom cache.

**```int Expiration```**
Default expiration in milliseconds

### Methods
```string ItemKey<T>(string id)```
Generates key for cache item of type T. Returns ```domain:collection:id``` where

```domain``` refers to ```Cache.Domain```
```collection``` refers to the type name or the parameter of ```CanBeCached``` attribute.

**Example:** ```User Cache:public-profile:00001```

```string ItemKey(object obj)```
Same as ```ItemKey<T>(string id)``` where collection name and id are derived from ```object``` using ```CanBeCached``` & ```CacheItem``` attributes

```KeyValuePair<RedisKey, RedisValue> GetKeyValuePair(object obj)```
Method to create Redis Key Value pair from the ```object```

**Example:**
```c#  
public class UserCache : RedisCache, IUserCache  
{  
    // ...  
      
    public async Task<bool> SetUserPublicProfileAsync(PublicUserProfile profile, int ttl)  
    {  
        var kvp = GetKeyValuePair(profile);  
        return await Database.StringSetAsync(kvp.Key, kvp.Value, TimeSpan.FromMilliseconds(ttl));   
    }  
  
    // ...  
}  
```  

## Extensions
### AddRedisCacheService
Extension to ```IServiceCollection``` for adding Redis service

```c#
	// App startup
	services.AddRedisCacheService("localhost:6379");
```

