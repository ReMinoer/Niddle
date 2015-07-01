using System;

namespace Diese.Injection
{
    public interface IDependencyInjector
    {
        T Resolve<T>();
        object Resolve(Type type);

        T ResolveKeyed<T>(object key);
        object ResolveKeyed(Type type, object key);
    }
}