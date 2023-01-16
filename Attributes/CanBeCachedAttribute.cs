namespace Moneo.Redis.Attributes;
/// <summary>
/// Attribute to define class or struct as an entity that may be cached.
/// One of the properties must be tagged with CacheKey Attribute
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class CanBeCachedAttribute : Attribute
{
    /// <summary>
    /// If specified, this value will be used as key prefix
    /// </summary>
    public string? StoreAs { get; set; }

    /// <summary>
    /// Attribute to define class or struct as an entity that may be cached.
    /// One of the properties must be tagged with CacheKey Attribute
    /// </summary>
    public CanBeCachedAttribute()
    {
        StoreAs = null;
    }
    /// <summary>
    /// Attribute to define class or struct as an entity that may be cached.
    /// One of the properties must be tagged with CacheKey Attribute 
    /// </summary>
    /// <param name="storeAs">If specified, this value will be used as key prefix</param>
    public CanBeCachedAttribute(string storeAs)
    {
        StoreAs = storeAs;
    }
}