using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Niddle.Base;

namespace Niddle
{
    public class InjectableManyAttribute : InjectableManyAttributeBase
    {
        private readonly string _addMethodName;

        public InjectableManyAttribute(string addMethodName = nameof(ICollection<object>.Add))
        {
            _addMethodName = addMethodName;
        }

        public override IEnumerable<Type> GetInjectableTypes(Type memberType)
        {
            return GetAddMethods(memberType).Select(x => x.GetParameters()[0].ParameterType);
        }

        public override sealed void Inject(PropertyInfo propertyInfo, object obj, object value)
        {
            GetBestAddMethod(propertyInfo.PropertyType, value.GetType()).Invoke(propertyInfo.GetValue(obj), new[] { value });
        }

        public override sealed void Inject(FieldInfo fieldInfo, object obj, object value)
        {
            GetBestAddMethod(fieldInfo.FieldType, value.GetType()).Invoke(fieldInfo.GetValue(obj), new[] { value });
        }

        private IEnumerable<MethodInfo> GetAddMethods(Type type)
        {
            return type.GetRuntimeMethods().Where(IsAddMethod);
        }

        private IEnumerable<MethodInfo> GetAddMethods(Type type, Type itemType)
        {
            return type.GetRuntimeMethods().Where(x => IsAddMethod(x) && x.GetParameters()[0].ParameterType.GetTypeInfo().IsAssignableFrom(itemType.GetTypeInfo()));
        }

        private MethodInfo GetBestAddMethod(Type type, Type itemType)
        {
            return GetAddMethods(type, itemType).Aggregate((x,y) => x.GetParameters()[0].ParameterType.GetTypeInfo().IsAssignableFrom(y.GetParameters()[0].ParameterType.GetTypeInfo()) ? y : x);
        }

        private bool IsAddMethod(MethodInfo methodInfo)
        {
            return methodInfo.Name == _addMethodName && methodInfo.GetParameters().Length == 1;
        }
    }
}