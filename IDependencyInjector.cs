using System;

namespace Diese.Injection
{
    public interface IDependencyInjector
    {
        T Resolve<T>(object key = null);
        object Resolve(Type type, object key = null);
    }
}