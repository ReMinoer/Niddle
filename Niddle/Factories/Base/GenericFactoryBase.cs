using System;
using System.Collections.Generic;

namespace Niddle.Factories.Base
{
    public abstract class GenericFactoryBase : FactoryBase, IGenericFactory
    {
        protected GenericFactoryBase(Type type, object serviceKey, Substitution substitution)
            : base(type, serviceKey, substitution)
        {
        }

        public abstract IDependencyFactory GetFactory(IEnumerable<Type> genericTypeArguments);
    }
}