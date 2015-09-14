using System;

namespace Diese.Injection
{
    public interface IDependencyFactory
    {
        Type Type { get; }
        object ServiceKey { get; }
        Substitution Substitution { get; }
        object Get(IDependencyInjector injector);
    }
}