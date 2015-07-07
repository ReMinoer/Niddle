using System;

namespace Diese.Injection
{
    public interface IDependencyInjector
    {
        T Resolve<T>(object serviceKey = null);
        object Resolve(Type type, object serviceKey = null);
    }
}