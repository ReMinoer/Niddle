using System;
using System.Linq;
using System.Reflection;

namespace Diese.Injection.Factories.Data
{
    internal class ParameterData
    {
        public Type Type { get; private set; }
        public object ServiceKey { get; private set; }
        public InjectableAttributeBase InjectableAttribute { get; private set; }
        public bool HasDefaultValue { get; private set; }
        public object DefaultValue { get; private set; }

        public ParameterData(ParameterInfo parameterInfo)
        {
            Type = parameterInfo.ParameterType;

            Attribute[] attributes = parameterInfo.GetCustomAttributes(typeof(ServiceKeyAttribute)).ToArray();
            if (attributes.Length > 0)
                ServiceKey = ((ServiceKeyAttribute)attributes.First()).Key;

            InjectableAttribute = parameterInfo.GetCustomAttribute<InjectableAttributeBase>();

            HasDefaultValue = parameterInfo.HasDefaultValue;
            if (HasDefaultValue)
                DefaultValue = parameterInfo.DefaultValue;
        }
    }
}