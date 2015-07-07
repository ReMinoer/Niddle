using System;

namespace Diese.Injection
{
    public class DependencyInjector : IDependencyInjector
    {
        private readonly IDependencyRegistry _registry;

        public DependencyInjector(IDependencyRegistry registry)
        {
            _registry = registry;
        }

        public T Resolve<T>(object serviceKey = null)
        {
            return (T)Resolve(typeof(T), serviceKey);
        }

        public object Resolve(Type type, object serviceKey = null)
        {
            return _registry[type, serviceKey].Get(this);
        }
    }
}