using System;
using Diese.Injection.Factories.Base;

namespace Diese.Injection.Factories
{
    internal class LinkedGenericFactory : GenericFactoryBase
    {
        private readonly IGenericFactory _factory;
        public override InstanceOrigin InstanceOrigin => _factory.InstanceOrigin;

        public LinkedGenericFactory(Type type, IGenericFactory factory, object serviceKey, Substitution substitution)
            : base(type, serviceKey, substitution)
        {
            _factory = factory;
        }

        public override IDependencyFactory GetFactory(Type[] genericTypeArguments)
        {
            return _factory.GetFactory(genericTypeArguments);
        }
    }
}