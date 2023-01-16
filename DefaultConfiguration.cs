using System.Globalization;
using Newtonsoft.Json;

namespace Moneo.Redis;

/// <summary>
/// Default RedisCache base class configuration
/// </summary>
public static class DefaultConfiguration
{
    /// <summary>
    /// Defaults to 0
    /// </summary>
    public const int Database = 0;
    /// <summary>
    /// Defaults to 5000 milliseconds
    /// </summary>
    public const int Expiration = 5 * 1000;
    /// <summary>
    /// Default Key part separator character
    /// </summary>
    public const char Separator = ':';

    /// <summary>
    /// Default JsonSerializer settings
    /// </summary>
    public static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
    {
        NullValueHandling = NullValueHandling.Ignore,
        Culture = CultureInfo.InvariantCulture
    };
}