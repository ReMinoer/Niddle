using System;
using System.Collections.Generic;
using Niddle.Injectables.Expressions;
using Niddle.Resolvables;

namespace Niddle.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class PopulatableAttribute : ResolvableAttribute
    {
        public string PopulateMethodName { get; set; } = nameof(ICollection<object>.Add);

        public override IInjectionExpression GetInjectionScenario(Type memberType) => new PopulateInjection(memberType.GetPopulateMethod(PopulateMethodName, Type));

        public override IResolvable GetResolvable(Type memberType, Attribute[] attributes) => new Populatable
        {
            PopulateMethodName = PopulateMethodName,
            Type = Type ?? memberType,
            Key = Key,
            InstanceOrigins = InstanceOrigins,
            AdditionalArguments = attributes
        };
    }
}