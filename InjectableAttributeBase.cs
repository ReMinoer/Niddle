using System;
using System.Collections;
using System.Reflection;

namespace Diese.Injection
{
    public interface IInjectableAttribute
    {
        void Inject(PropertyInfo propertyInfo, object obj, object value);
        void Inject(FieldInfo fieldInfo, object obj, object value);
    }

    public interface IInjectableManyAttribute : IInjectableAttribute
    {
        Type GetInjectedType(Type memberType);
        void Inject(PropertyInfo propertyInfo, object obj, IEnumerable values);
        void Inject(FieldInfo fieldInfo, object obj, IEnumerable values);
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public abstract class InjectableAttributeBase : Attribute, IInjectableAttribute
    {
        public abstract void Inject(PropertyInfo propertyInfo, object obj, object value);
        public abstract void Inject(FieldInfo fieldInfo, object obj, object value);
    }

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