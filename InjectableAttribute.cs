using System;

namespace Diese.Injection
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InjectableAttribute : Attribute
    {
    }
}