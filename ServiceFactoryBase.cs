using System;

namespace Diese.Injection
{
    internal abstract class ServiceFactoryBase : IServiceFactory
    {
        public Type Type { get; private set; }
        public object ServiceKey { get; private set; }
        public Substitution Substitution { get; private set; }

        protected ServiceFactoryBase(Type type, object serviceKey, Substitution substitution)
        {
            Type = type;
            ServiceKey = serviceKey;
            Substitution = substitution;
        }
    }
}