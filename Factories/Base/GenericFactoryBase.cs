using System;

namespace Diese.Injection.Factories.Base
{
    internal abstract class GenericFactoryBase : FactoryBase, IGenericFactory
    {
        protected GenericFactoryBase(Type type, object serviceKey, Substitution substitution)
            : base(type, serviceKey, substitution)
        {
        }

        public abstract IDependencyFactory GetFactory(Type[] genericTypeArguments);
    }
}