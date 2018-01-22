using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Diese.Injection.Base
{
    public abstract class DependencyInjectorBase : IDependencyInjector
    {
        public abstract object Resolve(Type type, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null);
        public abstract bool TryResolve(out object obj, Type type, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null);

        public T Resolve<T>(InjectableAttributeBase injectableAttribute = null, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null)
        {
            return (T)Resolve(typeof(T), injectableAttribute, serviceKey, instanceOrigins, dependencyInjector ?? this);
        }

        public IEnumerable<T> ResolveMany<T>(InjectableAttributeBase injectableAttribute = null, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null)
        {
            return ResolveMany(typeof(T), injectableAttribute, serviceKey, instanceOrigins, dependencyInjector ?? this).Cast<T>();
        }

        public virtual IEnumerable ResolveMany(Type type, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null)
        {
            yield return Resolve(type, injectableAttribute, serviceKey, instanceOrigins, dependencyInjector ?? this);
        }

        public bool TryResolve<T>(out T obj, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null)
        {
            if (TryResolve(out object temp, typeof(T), injectableAttribute, serviceKey, instanceOrigins, dependencyInjector ?? this))
            {
                obj = (T)temp;
                return true;
            }

            obj = default(T);
            return false;
        }

        public bool TryResolveMany<T>(out IEnumerable<T> objs, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null)
        {
            bool result = TryResolve(out T obj, injectableAttribute, serviceKey, instanceOrigins, dependencyInjector ?? this);
            objs = new[] { obj };
            return result;
        }

        public virtual bool TryResolveMany(out IEnumerable objs, Type type, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null)
        {
            bool result = TryResolve(out object obj, injectableAttribute, serviceKey, instanceOrigins, dependencyInjector ?? this);
            objs = new[] { obj };
            return result;
        }
    }
}