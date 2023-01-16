using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Moneo.Redis.Extensions;

public static class CacheServiceExtensions
{
    /// <summary>
    /// Adds StackExchange.Redis to the service container
    /// </summary>
    /// <param name="collection">IServiceCollection extension</param>
    /// <param name="configuration">Connection string for the Redis Service</param>
    public static void AddRedisCacheService(this IServiceCollection collection, string configuration)
    {
        collection.AddSingleton<IConnectionMultiplexer>(x =>
            ConnectionMultiplexer.Connect(configuration));
    }
}