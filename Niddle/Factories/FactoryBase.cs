using System;

namespace Niddle.Factories
{
    public abstract class FactoryBase : IInjectionService
    {
        public Type Type { get; }
        public object ServiceKey { get; }
        public Substitution Substitution { get; }
        public abstract InstanceOrigin? InstanceOrigin { get; }

        protected FactoryBase(Type type, object serviceKey, Substitution substitution)
        {
            Type = type;
            ServiceKey = serviceKey;
            Substitution = substitution;
        }
    }
}