using System;
using System.Collections;
using System.Reflection;

namespace Diese.Injection
{
    public interface IInjectableManyAttribute : IInjectableAttribute
    {
        Type GetInjectedType(Type memberType);
        void Inject(PropertyInfo propertyInfo, object obj, IEnumerable values);
        void Inject(FieldInfo fieldInfo, object obj, IEnumerable values);
    }
}