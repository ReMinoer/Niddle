using System;
using System.Collections;
using System.Reflection;

namespace Diese.Injection.Base
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public abstract class InjectableManyAttributeBase : InjectableAttributeBase, IInjectableManyAttribute
    {
        public abstract Type GetInjectedType(Type memberType);

        public void Inject(PropertyInfo propertyInfo, object obj, IEnumerable values)
        {
            foreach (object value in values)
                Inject(propertyInfo, obj, value);
        }

        public void Inject(FieldInfo fieldInfo, object obj, IEnumerable values)
        {
            foreach (object value in values)
                Inject(fieldInfo, obj, value);
        }
    }
}