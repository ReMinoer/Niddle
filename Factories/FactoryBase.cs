using System;

namespace Diese.Injection.Factories
{
    internal abstract class FactoryBase : IInjectionService
    {
        public Type Type { get; private set; }
        public object ServiceKey { get; private set; }
        public Substitution Substitution { get; private set; }

        protected FactoryBase(Type type, object serviceKey, Substitution substitution)
        {
            Type = type;
            ServiceKey = serviceKey;
            Substitution = substitution;
        }
    }
}