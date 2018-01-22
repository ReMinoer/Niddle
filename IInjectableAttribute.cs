using System.Reflection;

namespace Diese.Injection
{
    public interface IInjectableAttribute
    {
        void Inject(PropertyInfo propertyInfo, object obj, object value);
        void Inject(FieldInfo fieldInfo, object obj, object value);
    }
}