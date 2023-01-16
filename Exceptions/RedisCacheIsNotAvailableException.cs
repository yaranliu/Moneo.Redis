namespace Moneo.Redis.Exceptions;

public class RedisCacheIsNotAvailableException : Exception
{
    public RedisCacheIsNotAvailableException()
        : base("Cannot find Redis Cache Service")
    {
        
    }
}