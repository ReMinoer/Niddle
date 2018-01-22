using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Diese.Injection.Base;

namespace Diese.Injection
{
    public class InjectableCollectionAttribute : InjectableManyAttributeBase
    {
        public override Type GetInjectedType(Type memberType)
        {
            return memberType.GenericTypeArguments[0];
        }

        public override sealed void Inject(PropertyInfo propertyInfo, object obj, object value)
        {
            Type propertyType = propertyInfo.PropertyType;
            Type trackerType = TypePredicate(propertyType) ? propertyType : propertyType.GetInterfaces().First(TypePredicate);
            trackerType.GetMethod("Add").Invoke(propertyInfo.GetValue(obj), new[] { value });
        }

        public override sealed void Inject(FieldInfo fieldInfo, object obj, object value)
        {
            Type fieldType = fieldInfo.FieldType;
            Type trackerType = TypePredicate(fieldType) ? fieldType : fieldType.GetInterfaces().First(TypePredicate);
            trackerType.GetMethod("Add").Invoke(fieldInfo.GetValue(obj), new[] { value });
        }

        static private bool TypePredicate(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>);
        }
    }
}