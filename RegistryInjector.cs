using System;
using Diese.Injection.Base;

namespace Diese.Injection
{
    public class RegistryInjector : DependencyInjectorBase
    {
        public IDependencyRegistry Registry { get; }

        public RegistryInjector(IDependencyRegistry registry)
        {
            Registry = registry;
        }

        public override object Resolve(Type type, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null)
        {
            return Registry[type, serviceKey, instanceOrigins].Get(dependencyInjector ?? this);
        }

        public override bool TryResolve(out object obj, Type type, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null)
        {
            if (!Registry.TryGetFactory(out IDependencyFactory factory, type, serviceKey, instanceOrigins))
            {
                obj = null;
                return false;
            }

            obj = factory.Get(dependencyInjector ?? this);
            return true;
        }
    }
}