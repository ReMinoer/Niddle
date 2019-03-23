﻿using System;
using Niddle.Factories.Base;

namespace Niddle.Factories
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

        public override object Get(IDependencyInjector injector)
        {
            return _dependencyRegistry[_registeredType, _registeredKey].Get(injector);
        }
    }
}