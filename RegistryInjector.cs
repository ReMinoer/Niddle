using System;
using Diese.Injection.Exceptions;

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

        public object Resolve(Type type, object serviceKey = null)
        {
            object obj;
            if (TryResolve(out obj, type, serviceKey))
                return obj;

            throw new NotRegisterException(type, serviceKey);
        }

        public bool TryResolve<T>(out T obj, object serviceKey = null)
        {
            object obj2;
            if (TryResolve(out obj2, typeof(T), serviceKey))
            {
                obj = (T)obj2;
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