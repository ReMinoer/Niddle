using System;

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
            return (T)Resolve(typeof(T), serviceKey);
        }

        public virtual object Resolve(Type type, object serviceKey = null)
        {
            return Registry[type, serviceKey].Get(this);
        }

        public bool TryResolve<T>(out T obj, object serviceKey = null)
        {
            object temp;
            if (TryResolve(out temp, typeof(T), serviceKey))
            {
                obj = (T)temp;
                return true;
            }

            obj = default(T);
            return false;
        }

        public virtual bool TryResolve(out object obj, Type type, object serviceKey = null)
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
    }
}