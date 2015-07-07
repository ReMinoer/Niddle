using System;

namespace Diese.Injection.Factories
{
    internal class InstanceFactory : IDependencyFactory
    {
        private readonly object _instance;
        public Type Type { get; private set; }
        public object ServiceKey { get; private set; }

        public InstanceFactory(Type type, object instance, object serviceKey = null)
        {
            _instance = instance;

            Type = type;
            ServiceKey = serviceKey;
        }

        public object Get(IDependencyInjector injector)
        {
            return _instance;
        }
    }
}