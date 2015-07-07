using System;

namespace Diese.Injection
{
    public interface IDependencyFactory
    {
        Type Type { get; }
        object ServiceKey { get; }

        object Get(IDependencyInjector injector);
    }
}