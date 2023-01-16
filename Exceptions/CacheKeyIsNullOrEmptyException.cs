namespace Moneo.Redis.Exceptions;

public class CacheKeyIsNullOrEmptyException : Exception
{
    public CacheKeyIsNullOrEmptyException()
        : base("Cache: Object key cannot be null")
    {
        
    }
}