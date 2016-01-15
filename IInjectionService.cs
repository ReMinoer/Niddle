using System;

namespace Diese.Injection
{
    public interface IInjectionService
    {
        Type Type { get; }
        object ServiceKey { get; }
        Substitution Substitution { get; }
    }
}