using System.Reflection;
using Niddle.Injectables.Base;
using Niddle.Injectables.Expressions;

namespace Niddle.Injectables
{
    public class InjectableParameter : InjectableBase
    {
        public bool HasDefaultValue { get; }
        public object DefaultValue { get; }

        public InjectableParameter(IInjectionExpression injectionExpression, ParameterInfo parameterInfo)
            : base(injectionExpression, parameterInfo.ParameterType)
        {
            HasDefaultValue = parameterInfo.HasDefaultValue;
            if (HasDefaultValue)
                DefaultValue = parameterInfo.DefaultValue;
        }
    }
}