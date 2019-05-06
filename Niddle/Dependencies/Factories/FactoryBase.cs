using System;

namespace Niddle.Dependencies.Factories
{
    public abstract class FactoryBase : IFactory
    {
        public Type Type { get; }
        public object Key { get; }
        public Substitution Substitution { get; }
        public abstract InstanceOrigin? InstanceOrigin { get; }

        protected FactoryBase(Type type, object serviceKey, Substitution substitution)
        {
            Type = type;
            Key = serviceKey;
            Substitution = substitution;
        }
    }
}