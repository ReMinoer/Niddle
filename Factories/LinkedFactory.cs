using System;

namespace Diese.Injection.Factories
{
    internal class LinkedFactory : FactoryBase
    {
        private readonly IDependencyFactory _factory;

        public LinkedFactory(Type type, IDependencyFactory factory, object serviceKey, Substitution substitution)
            : base(type, serviceKey, substitution)
        {
            _factory = factory;
        }

        public override object Get(IDependencyInjector injector)
        {
            return _factory.Get(injector);
        }
    }
}