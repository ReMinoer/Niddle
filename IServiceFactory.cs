using System;

namespace Diese.Injection
{
    public interface IServiceFactory
    {
        Type Type { get; }
        object ServiceKey { get; }
        Substitution Substitution { get; }
    }
}