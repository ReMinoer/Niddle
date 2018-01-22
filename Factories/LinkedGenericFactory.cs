using System;
using Diese.Injection.Factories.Base;

namespace Diese.Injection.Factories
{
    internal class LinkedGenericFactory : GenericFactoryBase
    {
        private readonly DependencyRegistry.KeyableServiceRegistry<IGenericFactory> _genericFactories;
        private readonly Type _registeredTypeDescription;
        private readonly object _registeredKey;
        public override InstanceOrigin? InstanceOrigin => null;

        public LinkedGenericFactory(DependencyRegistry.KeyableServiceRegistry<IGenericFactory> genericFactories, Type linkedTypeDescription, Type registeredTypeDescription, object registeredKey, object serviceKey, Substitution substitution)
            : base(linkedTypeDescription, serviceKey, substitution)
        {
            _genericFactories = genericFactories;
            _registeredTypeDescription = registeredTypeDescription;
            _registeredKey = registeredKey;
        }

        public override IDependencyFactory GetFactory(Type[] genericTypeArguments)
        {
            return _genericFactories[_registeredTypeDescription, _registeredKey].GetFactory(genericTypeArguments);
        }
    }
}