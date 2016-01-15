﻿using System;

namespace Diese.Injection.Factories
{
    internal abstract class DependencyFactoryBase : ServiceFactoryBase, IDependencyFactory
    {
        protected DependencyFactoryBase(Type type, object serviceKey, Substitution substitution)
            : base(type, serviceKey, substitution)
        {
        }

        public abstract object Get(IDependencyInjector injector);
    }
}