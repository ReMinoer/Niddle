using System;
using System.Linq;
using System.Reflection;
using Niddle.Base;

namespace Niddle.Factories.Data
{
    public class ParameterData
    {
        public Type Type { get; private set; }
        public object ServiceKey { get; private set; }
        public InjectableAttributeBase InjectableAttribute { get; private set; }
        public bool HasDefaultValue { get; private set; }
        public object DefaultValue { get; private set; }

        public ParameterData(ParameterInfo parameterInfo)
        {
            Type = parameterInfo.ParameterType;
            
            InjectableAttribute = parameterInfo.GetCustomAttribute<InjectableAttributeBase>();
            ServiceKey = InjectableAttribute?.Key;

            HasDefaultValue = parameterInfo.HasDefaultValue;
            if (HasDefaultValue)
                DefaultValue = parameterInfo.DefaultValue;
        }
    }
}