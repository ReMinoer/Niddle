using System;
using System.Collections;
using System.Collections.Generic;

namespace Diese.Injection
{
    public class RegistryInjector : IDependencyInjector
    {
        public IDependencyRegistry Registry { get; private set; }

        public RegistryInjector(IDependencyRegistry registry)
        {
            Registry = registry;
        }

        public T Resolve<T>(object serviceKey = null)
        {
            return (T)Resolve(typeof(T), null, serviceKey);
        }

        public T Resolve<T>(InjectableAttributeBase injectableAttribute, object serviceKey = null)
        {
            return (T)Resolve(typeof(T), injectableAttribute, serviceKey);
        }

        public object Resolve(Type type, object serviceKey = null)
        {
            return Resolve(type, null, serviceKey);
        }
        
        public virtual object Resolve(Type type, InjectableAttributeBase injectableAttribute, object serviceKey = null)
        {
            return Registry[type, serviceKey].Get(this);
        }

        public IEnumerable ResolveMany(Type type, object serviceKey = null)
        {
            yield return Resolve(type, serviceKey);
        }

        public virtual IEnumerable ResolveMany(Type type, InjectableAttributeBase injectableAttribute, object serviceKey = null)
        {
            yield return Resolve(type, injectableAttribute, serviceKey);
        }

        public bool TryResolve<T>(out T obj, object serviceKey = null)
        {
            object temp;
            if (TryResolve(out temp, typeof(T), null, serviceKey))
            {
                obj = (T)temp;
                return true;
            }

            obj = default(T);
            return false;
        }

        public bool TryResolve<T>(out T obj, InjectableAttributeBase injectableAttribute, object serviceKey = null)
        {
            object temp;
            if (TryResolve(out temp, typeof(T), injectableAttribute, serviceKey))
            {
                obj = (T)temp;
                return true;
            }

            obj = default(T);
            return false;
        }

        public bool TryResolve(out object obj, Type type, object serviceKey = null)
        {
            return TryResolve(out obj, type, null, serviceKey);
        }

        public virtual bool TryResolve(out object obj, Type type, InjectableAttributeBase injectableAttribute, object serviceKey = null)
        {
            IDependencyFactory factory;
            if (!Registry.TryGetFactory(out factory, type, serviceKey))
            {
                obj = null;
                return false;
            }

            obj = factory.Get(this);
            return true;
        }

        public bool TryResolveMany<T>(out IEnumerable<T> objs, object serviceKey = null)
        {
            bool result = TryResolve(out T obj, serviceKey);
            objs = new [] {obj};
            return result;
        }

        public bool TryResolveMany<T>(out IEnumerable<T> objs, InjectableAttributeBase injectableAttribute, object serviceKey = null)
        {
            bool result = TryResolve(out T obj, injectableAttribute, serviceKey);
            objs = new[] { obj };
            return result;
        }

        public bool TryResolveMany(out IEnumerable objs, Type type, object serviceKey = null)
        {
            bool result = TryResolve(out object obj, serviceKey);
            objs = new[] { obj };
            return result;
        }

        public virtual bool TryResolveMany(out IEnumerable objs, Type type, InjectableAttributeBase injectableAttribute, object serviceKey = null)
        {
            bool result = TryResolve(out object obj, injectableAttribute, serviceKey);
            objs = new[] { obj };
            return result;
        }
    }
}