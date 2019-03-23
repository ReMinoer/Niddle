using System;
using Niddle.Factories.Data;

namespace Niddle.Factories
{
    public class SingletonFactory : NewInstanceFactory
    {
        private object _instance;
        public override InstanceOrigin? InstanceOrigin => Niddle.InstanceOrigin.Registration;

        public SingletonFactory(Type type, object serviceKey, ConstructorData constructorData, Substitution substitution)
            : base(type, serviceKey, constructorData, substitution)
        {
        }

        public override object Get(IDependencyInjector injector)
        {
            if (_instance != null)
                return _instance;

            _instance = base.Get(injector);

            return _instance;
        }
    }
}