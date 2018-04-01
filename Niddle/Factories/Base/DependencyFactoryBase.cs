using System;

namespace Niddle.Factories.Base
{
    internal abstract class DependencyFactoryBase : FactoryBase, IDependencyFactory
    {
        protected DependencyFactoryBase(Type type, object serviceKey, Substitution substitution)
            : base(type, serviceKey, substitution)
        {
        }

        public abstract object Get(IDependencyInjector injector);
    }
}