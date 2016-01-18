using System;
using Diese.Injection.Factories.Base;

namespace Diese.Injection.Factories
{
    internal class InstanceFactory : DependencyFactoryBase
    {
        private readonly object _instance;

        public InstanceFactory(Type type, object instance, object serviceKey, Substitution substitution)
            : base(type, serviceKey, substitution)
        {
            _instance = instance;
        }

        public override object Get(IDependencyInjector injector)
        {
            return _instance;
        }
    }
}