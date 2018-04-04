using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Niddle.Utils;

namespace Niddle.Base
{
    public abstract class InjectableManyByGenericMethodAttributeBase : InjectableManyAttributeBase
    {
        protected abstract Type GenericTypeDefinition { get; }
        protected abstract string MethodName { get; }

        public override IEnumerable<Type> GetInjectableTypes(Type memberType)
        {
            yield return memberType.GenericTypeArguments[0];
        }

        public override sealed void Inject(PropertyInfo propertyInfo, object obj, object value)
        {
            GetGenericType(propertyInfo.PropertyType).GetTypeInfo().GetDeclaredMethod(MethodName).Invoke(propertyInfo.GetValue(obj), new[] { value });
        }

        public override sealed void Inject(FieldInfo fieldInfo, object obj, object value)
        {
            GetGenericType(fieldInfo.FieldType).GetTypeInfo().GetDeclaredMethod(MethodName).Invoke(fieldInfo.GetValue(obj), new[] { value });
        }

        private Type GetGenericType(Type memberType)
        {
            if (IsMatchingGenericTypeDefinition(memberType))
                return memberType;

            if (GenericTypeDefinition.GetTypeInfo().IsInterface)
                return memberType.GetTypeInfo().ImplementedInterfaces.First(IsMatchingGenericTypeDefinition);

            return memberType.GetBaseTypesExclusive().First(IsMatchingGenericTypeDefinition);
        }

        private bool IsMatchingGenericTypeDefinition(Type type)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == GenericTypeDefinition;
        }
    }
}