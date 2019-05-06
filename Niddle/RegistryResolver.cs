using System;
using System.Collections.Generic;
using Niddle.Base;

namespace Niddle
{
    public class RegistryResolver : DependencyResolverBase
    {
        public IDependencyRegistry Registry { get; }

        public RegistryResolver(IDependencyRegistry registry)
        {
            Registry = registry;
        }

        public override object Resolve(Type type, object serviceKey = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null, IEnumerable<object> args = null)
        {
            return Registry[type, serviceKey, origins].Get(resolver ?? this);
        }

        public override bool TryResolve(out object obj, Type type, object serviceKey = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null, IEnumerable<object> args = null)
        {
            if (!Registry.TryGetFactory(out IDependencyFactory factory, type, serviceKey, origins))
            {
                obj = null;
                return false;
            }

            obj = factory.Get(resolver ?? this);
            return true;
        }
    }
}