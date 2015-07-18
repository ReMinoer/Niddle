using System;
using System.Linq;
using System.Reflection;

namespace Diese.Injection.Factories.Data
{
    internal class MemberData
    {
        public Type Type { get; private set; }
        public object ServiceKey { get; private set; }

        public MemberData(Type type, MemberInfo memberInfo)
        {
            Type = type;

            Attribute[] attributes = memberInfo.GetCustomAttributes(typeof(ServiceKeyAttribute)).ToArray();
            if (attributes.Length > 0)
                ServiceKey = ((ServiceKeyAttribute)attributes.First()).Key;
        }
    }
}