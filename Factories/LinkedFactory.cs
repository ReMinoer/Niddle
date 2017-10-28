using System;
using Diese.Injection.Factories.Base;

namespace Diese.Injection.Factories
{
    internal class LinkedFactory : DependencyFactoryBase
    {
        private readonly IDependencyFactory _factory;
        public override InstanceOrigin InstanceOrigin => _factory.InstanceOrigin;

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