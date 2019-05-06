using System;
using System.Collections.Generic;
using System.Reflection;
using Niddle.Attributes.Base;

namespace Niddle
{
    static public class ResolvableMembersProvider
    {
        static public IEnumerable<IResolvableInjectable<TTarget, object>> Get<TTarget>()
        {
            return Get<TTarget>(typeof(TTarget));
        }

        static public IEnumerable<IResolvableInjectable<TTarget, object>> Get<TTarget>(Type type)
        {
            foreach (FieldInfo fieldInfo in type.GetRuntimeFields())
            {
                if (!fieldInfo.IsPublic)
                    continue;

                var attribute = fieldInfo.GetCustomAttribute<ResolvableAttributeBase>();
                if (attribute == null)
                    continue;

                yield return fieldInfo.AsResolvableInjectable<TTarget>();
            }

            foreach (PropertyInfo propertyInfo in type.GetRuntimeProperties())
            {
                if ((propertyInfo.SetMethod == null || !propertyInfo.SetMethod.IsPublic)
                    && (propertyInfo.GetMethod == null || !propertyInfo.GetMethod.IsPublic))
                    continue;

                var attribute = propertyInfo.GetCustomAttribute<ResolvableAttributeBase>();
                if (attribute == null)
                    continue;

                yield return propertyInfo.AsResolvableInjectable<TTarget>();
            }
        }
    }
}