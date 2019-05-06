using System;
using Niddle.Dependencies.Factories.Base;

namespace Niddle.Dependencies.Factories
{
    public class InstanceFactory : DependencyFactoryBase
    {
        private readonly object _instance;
        public override InstanceOrigin? InstanceOrigin => Niddle.InstanceOrigin.Registration;

        public InstanceFactory(Type type, object instance, object serviceKey, Substitution substitution)
            : base(type, serviceKey, substitution)
        {
            _instance = instance;
        }

        public override object Get(IDependencyResolver resolver)
        {
            return _instance;
        }
    }
}