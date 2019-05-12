using System;
using System.Collections;

namespace Niddle.Dependencies.Factories
{
    public class SingletonFactory : NewInstanceFactory
    {
        private object _instance;
        public override InstanceOrigin? InstanceOrigin => Niddle.InstanceOrigin.Registration;

        public SingletonFactory(Type type, object serviceKey, IResolvableRejecter<object, IEnumerable, IEnumerable, object> resolvableInstantiator, Substitution substitution,
                                IResolvableMembersProvider<object> resolvableMembersProvider = null)
            : base(type, serviceKey, resolvableInstantiator, substitution, resolvableMembersProvider)
        {
        }

        public override object Get(IDependencyResolver resolver)
        {
            if (_instance != null)
                return _instance;

            _instance = base.Get(resolver);

            return _instance;
        }
    }
}