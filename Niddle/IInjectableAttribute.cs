using System.Reflection;

namespace Niddle
{
    public interface IInjectableAttribute
    {
        void Inject(PropertyInfo propertyInfo, object obj, object value);
        void Inject(FieldInfo fieldInfo, object obj, object value);
    }
}