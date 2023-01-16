using Moneo.Redis.Exceptions;

namespace Moneo.Redis.Attributes;

/// <summary>
/// Helper class for resolving meta data created by CanBeCached and CacheKey attributes. 
/// </summary>
public static class AttributeResolver
{
    /// <summary>
    /// Method to check CanBeCached attribute. 
    /// </summary>
    /// <param name="obj">The instance of the object which will be checked for CanBeCached Attribute</param>
    /// <returns>Returns StoreAs if defined in the attribute, class or struct name otherwise.</returns>
    public static string GetCollectionName(object obj)
    {
        var attributes = System.Attribute.GetCustomAttributes(obj.GetType());
        foreach (var attr in attributes)
        {
            if (attr is CanBeCachedAttribute attribute)
            {
                return string.IsNullOrWhiteSpace(attribute.StoreAs) ? obj.GetType().Name : attribute.StoreAs;
            }
        }
        return obj.GetType().Name;
    }
    
    /// <summary>
    /// Method to check CanBeCached attribute.
    /// </summary>
    /// <typeparam name="T">Class or struct which will be checked for CanBeCached Attribute </typeparam>
    /// <returns>Returns StoreAs if defined in the attribute, class or struct name otherwise.</returns>
    public static string GetCollectionName<T>()
    {
        var attributes = System.Attribute.GetCustomAttributes(typeof(T));
        foreach (var attr in attributes)
        {
            if (attr is CanBeCachedAttribute attribute)
            {
                return string.IsNullOrWhiteSpace(attribute.StoreAs) ? typeof(T).Name : attribute.StoreAs;
            }
        }
        return typeof(T).Name;
    }
    /// <summary>
    /// Method to get cache key identifier denoted by CacheKey attribute
    /// </summary>
    /// <param name="obj">Object to check</param>
    /// <returns>Returns the value of the property denoted by CacheKey attribute</returns>
    /// <exception cref="CacheKeyIsNullOrEmptyException">If none of the object's properties has CacheKey attribute</exception>
    public static string GetKeyValue(object obj)
    {
        string? key = null;
        var properties = obj.GetType().GetProperties();
        foreach (var prop in properties)
        {
            var propAttributes = prop.GetCustomAttributes(true);
            foreach (var propAttribute in propAttributes)
            {
                if (propAttribute is not CacheKeyAttribute keyProp) continue;
                var value = prop.GetValue(obj);
                if (value != null) key += value.ToString();
            }
        }
        if (string.IsNullOrWhiteSpace(key)) throw new CacheKeyIsNullOrEmptyException();
        return key;
    }
}