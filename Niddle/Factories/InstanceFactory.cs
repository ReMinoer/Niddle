using System;
using Niddle.Factories.Base;

namespace Niddle.Factories
{
    internal class InstanceFactory : DependencyFactoryBase
    {
        private readonly object _instance;
        public override InstanceOrigin? InstanceOrigin => Niddle.InstanceOrigin.Registration;

        public InstanceFactory(Type type, object instance, object serviceKey, Substitution substitution)
            : base(type, serviceKey, substitution)
        {
            _instance = instance;
        }

        public override object Get(IDependencyInjector injector)
        {
            return _instance;
        }
    }
}