using System;

namespace Diese.Injection
{
    public interface IDependencyInjector
    {
        T Resolve<T>(object serviceKey = null);
        T Resolve<T>(InjectableAttribute injectableAttribute, object serviceKey = null);
        object Resolve(Type type, object serviceKey = null);
        object Resolve(Type type, InjectableAttribute injectableAttribute, object serviceKey = null);
        bool TryResolve<T>(out T obj, object serviceKey = null);
        bool TryResolve<T>(out T obj, InjectableAttribute injectableAttribute, object serviceKey = null);
        bool TryResolve(out object obj, Type type, object serviceKey = null);
        bool TryResolve(out object obj, Type type, InjectableAttribute injectableAttribute, object serviceKey = null);
    }
}