using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Niddle.Dependencies.Factories.Base;
using Niddle.Exceptions;

namespace Niddle.Dependencies.Factories
{
    public class NewInstanceFactory : DependencyFactoryBase
    {
        static private readonly Stack<IDependencyFactory> FactoryStack = new Stack<IDependencyFactory>();
        private bool _alreadyInvoke;
        private readonly IResolvableRejecter<object, IEnumerable, IEnumerable, object> _resolvableInstantiator;
        private readonly IResolvableMembersProvider<object> _resolvableMembersProvider;

        public override InstanceOrigin? InstanceOrigin => Niddle.InstanceOrigin.Instantiation;

        public NewInstanceFactory(Type type, object serviceKey, IResolvableRejecter<object, IEnumerable, IEnumerable, object> resolvableInstantiator, Substitution substitution,
                                  IResolvableMembersProvider<object> resolvableMembersProvider = null)
            : base(type, serviceKey, substitution)
        {
            _resolvableInstantiator = resolvableInstantiator;
            _resolvableMembersProvider = resolvableMembersProvider;
        }

        public override object Get(IDependencyResolver resolver)
        {
            if (_alreadyInvoke)
                throw new CyclicDependencyException(FactoryStack);

            FactoryStack.Push(this);
            _alreadyInvoke = true;

            object instance = _resolvableInstantiator.ResolveAndReject(resolver, null);

            IResolvableInjectable<object, object, object>[] resolvableInjectables = _resolvableMembersProvider?.ForType(instance.GetType()).ToArray();
            if (resolvableInjectables != null)
                foreach (IResolvableInjectable<object, object, object> resolvableMember in resolvableInjectables)
                    resolvableMember.TryResolveAndInject(resolver, instance);

            _alreadyInvoke = false;
            FactoryStack.Pop();

            return instance;
        }
    }
}