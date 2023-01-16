namespace Moneo.Redis.Attributes;
/// <summary>
/// Attribute to define the key of cached item 
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class CacheKeyAttribute : Attribute {}