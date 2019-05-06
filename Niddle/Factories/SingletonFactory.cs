using System;
using System.Collections;

namespace Niddle.Factories
{
    public class SingletonFactory : NewInstanceFactory
    {
        private object _instance;
        public override InstanceOrigin? InstanceOrigin => Niddle.InstanceOrigin.Registration;

        public SingletonFactory(Type type, object serviceKey, IResolvableRejecter<object, IEnumerable, object> resolvableInstantiator, Substitution substitution)
            : base(type, serviceKey, resolvableInstantiator, substitution)
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