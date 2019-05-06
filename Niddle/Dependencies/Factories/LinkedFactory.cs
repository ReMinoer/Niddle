using System;
using Niddle.Dependencies.Factories.Base;

namespace Niddle.Dependencies.Factories
{
    internal class LinkedFactory : DependencyFactoryBase
    {
        private readonly IDependencyRegistry _dependencyRegistry;
        private readonly Type _registeredType;
        private readonly object _registeredKey;
        public override InstanceOrigin? InstanceOrigin => null;

        public LinkedFactory(IDependencyRegistry dependencyRegistry, Type linkedType, Type registeredType, object registeredKey, object serviceKey, Substitution substitution)
            : base(linkedType, serviceKey, substitution)
        {
            _dependencyRegistry = dependencyRegistry;
            _registeredType = registeredType;
            _registeredKey = registeredKey;
        }

        public override object Get(IDependencyResolver resolver)
        {
            return _dependencyRegistry[_registeredType, _registeredKey].Get(resolver);
        }
    }
}