using System;
using System.Reflection;

namespace Diese.Injection.Factories
{
    internal class SingletonFactory : TransientFactory
    {
        private object _instance;

        public SingletonFactory(Type type, object serviceKey, ConstructorInfo constructorInfo, Substitution substitution)
            : base(type, serviceKey, constructorInfo, substitution)
        {
        }

        public override object Get(IDependencyInjector injector)
        {
            if (_instance != null)
                return _instance;

            _instance = base.Get(injector);

            return _instance;
        }
    }
}