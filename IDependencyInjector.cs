using System;
using System.Collections;
using System.Collections.Generic;
using Diese.Injection.Base;

namespace Diese.Injection
{
    public interface IDependencyInjector
    {
        T Resolve<T>(InjectableAttributeBase injectableAttribute = null, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null);
        object Resolve(Type type, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null);
        IEnumerable<T> ResolveMany<T>(InjectableAttributeBase injectableAttribute = null, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null);
        IEnumerable ResolveMany(Type type, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null);
        bool TryResolve<T>(out T obj, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null);
        bool TryResolve(out object obj, Type type, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null);
        bool TryResolveMany<T>(out IEnumerable<T> objs, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null);
        bool TryResolveMany(out IEnumerable objs, Type type, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null);
    }
}