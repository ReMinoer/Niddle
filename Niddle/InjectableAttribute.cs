using System.Reflection;
using Niddle.Base;

namespace Niddle
{
    public class InjectableAttribute : InjectableAttributeBase
    {
        public override void Inject(PropertyInfo propertyInfo, object obj, object value)
        {
            propertyInfo.SetValue(obj, value);
        }

        public override void Inject(FieldInfo fieldInfo, object obj, object value)
        {
            fieldInfo.SetValue(obj, value);
        }
    }
}