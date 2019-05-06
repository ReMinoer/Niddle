using System;
using System.Collections.Generic;
using Niddle.Base;

namespace Niddle
{
    public class RegistryInjector : DependencyInjectorBase
    {
        public IDependencyRegistry Registry { get; }

        public RegistryInjector(IDependencyRegistry registry)
        {
            Registry = registry;
        }

        public override object Resolve(Type type, object serviceKey = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null, IEnumerable<object> args = null)
        {
            return Registry[type, serviceKey, origins].Get(dependencyInjector ?? this);
        }

        public override bool TryResolve(out object obj, Type type, object serviceKey = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null, IEnumerable<object> args = null)
        {
            if (!Registry.TryGetFactory(out IDependencyFactory factory, type, serviceKey, origins))
            {
                obj = null;
                return false;
            }

            obj = factory.Get(dependencyInjector ?? this);
            return true;
        }
    }
}