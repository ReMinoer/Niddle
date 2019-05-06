using System;
using Niddle.Attributes.Base;
using Niddle.Injectables.Expressions;
using Niddle.Resolvables;

namespace Niddle.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Constructor | AttributeTargets.Method)]
    public class ResolvableAttribute : ResolvableAttributeBase
    {
        public Type Type { get; set; }
        public object Key { get; set; }
        public InstanceOrigins InstanceOrigins { get; set; } = InstanceOrigins.All;

        public override IInjectionExpression GetInjectionScenario(Type memberType) => new AssignementInjection();

        public override IResolvable GetResolvable(Type memberType, Attribute[] attributes) => new Resolvable
        {
            Type = Type ?? memberType,
            Key = Key,
            InstanceOrigins = InstanceOrigins,
            AdditionalArguments = attributes
        };

        public override sealed IResolvable<TValue> GetResolvable<TValue>(Type memberType, Attribute[] attributes) => new CastingResolvable<TValue>
        {
            Resolvable = GetResolvable(memberType, attributes)
        };
    }
}