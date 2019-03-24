using System;
using System.Reflection;

namespace Niddle.Base
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public abstract class InjectableAttributeBase : Attribute, IInjectableAttribute
    {
        public object Key { get; set; }
        public abstract void Inject(PropertyInfo propertyInfo, object obj, object value);
        public abstract void Inject(FieldInfo fieldInfo, object obj, object value);
    }
}