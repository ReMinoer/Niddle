using System;
using System.Reflection;

namespace Diese.Injection.Factories.Data
{
    internal class MemberData
    {
        public Type Type { get; private set; }
        public object ServiceKey { get; private set; }
        public InjectableAttribute InjectableAttribute { get; private set; }

        public MemberData(Type type, MemberInfo memberInfo)
        {
            Type = type;

            ServiceKey = memberInfo.GetCustomAttribute<ServiceKeyAttribute>()?.Key;
            InjectableAttribute = memberInfo.GetCustomAttribute<InjectableAttribute>();
        }
    }
}