using System;

namespace Diese.Injection
{
    public class RegistryInjector : IDependencyInjector
    {
        private readonly IDependencyRegistry _registry;

        public RegistryInjector(IDependencyRegistry registry)
        {
            _registry = registry;
        }

        public T Resolve<T>(object serviceKey = null)
        {
            return (T)Resolve(typeof(T), serviceKey);
        }

        public virtual object Resolve(Type type, object serviceKey = null)
        {
            return _registry[type, serviceKey].Get(this);
        }
    }
}