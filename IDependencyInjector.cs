using System;
using System.Collections;
using System.Collections.Generic;

namespace Diese.Injection
{
    public interface IDependencyInjector
    {
        T Resolve<T>(object serviceKey = null);
        T Resolve<T>(InjectableAttributeBase injectableAttribute, object serviceKey = null);
        object Resolve(Type type, object serviceKey = null);
        object Resolve(Type type, InjectableAttributeBase injectableAttribute, object serviceKey = null);
        IEnumerable ResolveMany(Type type, object serviceKey = null);
        IEnumerable ResolveMany(Type type, InjectableAttributeBase injectableAttribute, object serviceKey = null);
        bool TryResolve<T>(out T obj, object serviceKey = null);
        bool TryResolve<T>(out T obj, InjectableAttributeBase injectableAttribute, object serviceKey = null);
        bool TryResolve(out object obj, Type type, object serviceKey = null);
        bool TryResolve(out object obj, Type type, InjectableAttributeBase injectableAttribute, object serviceKey = null);
        bool TryResolveMany<T>(out IEnumerable<T> objs, object serviceKey = null);
        bool TryResolveMany<T>(out IEnumerable<T> objs, InjectableAttributeBase injectableAttribute, object serviceKey = null);
        bool TryResolveMany(out IEnumerable objs, Type type, object serviceKey = null);
        bool TryResolveMany(out IEnumerable objs, Type type, InjectableAttributeBase injectableAttribute, object serviceKey = null);
    }
}