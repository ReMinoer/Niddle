using System;

namespace Diese.Injection.Factories
{
    internal interface IDependencyFactory
    {
        Type Type { get; }
        object ServiceKey { get; }

        object Get(IDependencyInjector injector);
    }
}