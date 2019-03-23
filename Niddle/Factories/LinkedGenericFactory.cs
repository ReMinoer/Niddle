using System;
using System.Collections.Generic;
using Niddle.Factories.Base;

namespace Niddle.Factories
{
    internal class LinkedGenericFactory : GenericFactoryBase
    {
        private readonly IDependencyRegistry _dependencyRegistry;
        private readonly Type _registeredTypeDescription;
        private readonly object _registeredKey;
        public override InstanceOrigin? InstanceOrigin => null;

        public LinkedGenericFactory(IDependencyRegistry dependencyRegistry, Type linkedTypeDescription, Type registeredTypeDescription, object registeredKey, object serviceKey, Substitution substitution)
            : base(linkedTypeDescription, serviceKey, substitution)
        {
            _dependencyRegistry = dependencyRegistry;
            _registeredTypeDescription = registeredTypeDescription;
            _registeredKey = registeredKey;
        }

        public override IDependencyFactory GetFactory(IEnumerable<Type> genericTypeArguments)
        {
            return _dependencyRegistry[_registeredTypeDescription, genericTypeArguments, _registeredKey];
        }
    }
}