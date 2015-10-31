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
    }
}