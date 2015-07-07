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

        public T Resolve<T>(object key = null)
        {
            return (T)Resolve(typeof(T), key);
        }

        public object Resolve(Type type, object key = null)
        {
            return _registry[type, key].Get(this);
        }
    }
}