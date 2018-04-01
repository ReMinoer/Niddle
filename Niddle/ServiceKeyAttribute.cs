using System;

namespace Niddle
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    public class ServiceKeyAttribute : Attribute
    {
        public object Key { get; set; }

        public ServiceKeyAttribute(object key)
        {
            Key = key;
        }
    }
}