using System;
using System.Collections.Generic;
using System.Reflection;

namespace Niddle.Base
{
    public abstract class InjectableManyByMethodAttributeBase : InjectableManyAttributeBase
    {
        protected abstract MethodInfo MethodInfo { get; }
        
        public override IEnumerable<Type> GetInjectableTypes(Type memberType)
        {
            yield return MethodInfo.GetParameters()[0].ParameterType;
        }

        public override void Inject(PropertyInfo propertyInfo, object obj, object value)
        {
            MethodInfo.Invoke(propertyInfo.GetValue(obj), new[] { value });
        }

        public override void Inject(FieldInfo fieldInfo, object obj, object value)
        {
            MethodInfo.Invoke(fieldInfo.GetValue(obj), new[] { value });
        }
    }
}