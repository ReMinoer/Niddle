using System;

namespace Diese.Injection
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class InjectableAttribute : Attribute
    {
    }
}