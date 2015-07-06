using System;

namespace Diese.Injection.Factories.Data
{
    internal struct ParameterData
    {
        public Type Type;
        public object ServiceKey;

        public ParameterData(Type type, object serviceKey)
        {
            Type = type;
            ServiceKey = serviceKey;
        }
    }
}