using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Diese.Injection
{
    public class RegistryInjector : IDependencyInjector
    {
        public IDependencyRegistry Registry { get; private set; }

        public RegistryInjector(IDependencyRegistry registry)
        {
            Registry = registry;
        }

        public T Resolve<T>(InjectableAttributeBase injectableAttribute = null, object serviceKey = null, IDependencyInjector dependencyInjector = null)
        {
            return (T)Resolve(typeof(T), injectableAttribute, serviceKey, dependencyInjector ?? this);
        }
        
        public virtual object Resolve(Type type, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, IDependencyInjector dependencyInjector = null)
        {
            return Registry[type, serviceKey].Get(dependencyInjector ?? this);
        }

        public IEnumerable<T> ResolveMany<T>(InjectableAttributeBase injectableAttribute = null, object serviceKey = null, IDependencyInjector dependencyInjector = null)
        {
            return ResolveMany(typeof(T), injectableAttribute, serviceKey, dependencyInjector ?? this).Cast<T>();
        }

        public virtual IEnumerable ResolveMany(Type type, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, IDependencyInjector dependencyInjector = null)
        {
            yield return Resolve(type, injectableAttribute, serviceKey, dependencyInjector ?? this);
        }

        public bool TryResolve<T>(out T obj, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, IDependencyInjector dependencyInjector = null)
        {
            object temp;
            if (TryResolve(out temp, typeof(T), injectableAttribute, serviceKey, dependencyInjector ?? this))
            {
                obj = (T)temp;
                return true;
            }

            obj = default(T);
            return false;
        }

        public virtual bool TryResolve(out object obj, Type type, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, IDependencyInjector dependencyInjector = null)
        {
            IDependencyFactory factory;
            if (!Registry.TryGetFactory(out factory, type, serviceKey))
            {
                obj = null;
                return false;
            }

            obj = factory.Get(dependencyInjector ?? this);
            return true;
        }

        public bool TryResolveMany<T>(out IEnumerable<T> objs, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, IDependencyInjector dependencyInjector = null)
        {
            bool result = TryResolve(out T obj, injectableAttribute, serviceKey, dependencyInjector ?? this);
            objs = new[] { obj };
            return result;
        }

        public virtual bool TryResolveMany(out IEnumerable objs, Type type, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, IDependencyInjector dependencyInjector = null)
        {
            bool result = TryResolve(out object obj, injectableAttribute, serviceKey, dependencyInjector ?? this);
            objs = new[] { obj };
            return result;
        }
    }
}