﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Niddle.Exceptions;
using Niddle.Factories.Base;

namespace Niddle.Factories
{
    public class NewInstanceFactory : DependencyFactoryBase
    {
        static private readonly Stack<IDependencyFactory> FactoryStack = new Stack<IDependencyFactory>();
        private bool _alreadyInvoke;
        private readonly IResolvableRejecter<object, IEnumerable, object> _resolvableInstantiator;
        private readonly IResolvableInjectable<object, object>[] _resolvableMembers;
        public override InstanceOrigin? InstanceOrigin => Niddle.InstanceOrigin.Instantiation;

        public NewInstanceFactory(Type type, object serviceKey, IResolvableRejecter<object, IEnumerable, object> resolvableInstantiator, Substitution substitution)
            : base(type, serviceKey, substitution)
        {
            _resolvableInstantiator = resolvableInstantiator;
            _resolvableMembers = ResolvableMembersProvider.Get<object>(type).ToArray();
        }

        public override object Get(IDependencyInjector injector)
        {
            if (_alreadyInvoke)
                throw new CyclicDependencyException(FactoryStack);

            FactoryStack.Push(this);
            _alreadyInvoke = true;

            object instance = _resolvableInstantiator.ResolveAndReject(injector, null);

            foreach (IResolvableInjectable<object, object> resolvableMember in _resolvableMembers)
                resolvableMember.TryResolveAndInject(injector, instance);

            _alreadyInvoke = false;
            FactoryStack.Pop();

            return instance;
        }
    }
}