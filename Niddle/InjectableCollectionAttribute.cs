using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Niddle.Base;

namespace Niddle
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
            Type trackerType = TypePredicate(propertyType) ? propertyType : propertyType.GetTypeInfo().ImplementedInterfaces.First(TypePredicate);
            trackerType.GetTypeInfo().GetDeclaredMethod("Add").Invoke(propertyInfo.GetValue(obj), new[] { value });
        }

        public override sealed void Inject(FieldInfo fieldInfo, object obj, object value)
        {
            Type fieldType = fieldInfo.FieldType;
            Type trackerType = TypePredicate(fieldType) ? fieldType : fieldType.GetTypeInfo().ImplementedInterfaces.First(TypePredicate);
            trackerType.GetTypeInfo().GetDeclaredMethod("Add").Invoke(fieldInfo.GetValue(obj), new[] { value });
        }

        static private bool TypePredicate(Type type)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>);
        }
    }
}