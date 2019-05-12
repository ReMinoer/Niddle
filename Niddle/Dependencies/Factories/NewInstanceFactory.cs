﻿using System;
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
        private readonly IResolvableInjectable<object, object, object>[] _resolvableMembers;
        public override InstanceOrigin? InstanceOrigin => Niddle.InstanceOrigin.Instantiation;

        public NewInstanceFactory(Type type, object serviceKey, IResolvableRejecter<object, IEnumerable, IEnumerable, object> resolvableInstantiator, Substitution substitution,
                                  IResolvableMembersProvider<object> resolvableMembersProvider = null)
            : base(type, serviceKey, substitution)
        {
            _resolvableInstantiator = resolvableInstantiator;
            _resolvableMembers = resolvableMembersProvider?.ForType(type).ToArray();
        }

        public override object Get(IDependencyResolver resolver)
        {
            if (_alreadyInvoke)
                throw new CyclicDependencyException(FactoryStack);

            FactoryStack.Push(this);
            _alreadyInvoke = true;

            object instance = _resolvableInstantiator.ResolveAndReject(resolver, null);

            if (_resolvableMembers != null)
                foreach (IResolvableInjectable<object, object, object> resolvableMember in _resolvableMembers)
                    resolvableMember.TryResolveAndInject(resolver, instance);

            _alreadyInvoke = false;
            FactoryStack.Pop();

            return instance;
        }
    }
}