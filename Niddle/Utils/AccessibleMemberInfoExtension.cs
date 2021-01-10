using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Niddle.Utils
{
    static public class AccessibleMemberInfoExtension
    {
        static public IEnumerable<FieldInfo> GetAccessibleFields(this Type type)
        {
            return type.GetRuntimeFields().Where(IsPubliclyAccessible);
        }

        static public IEnumerable<PropertyInfo> GetAccessibleProperties(this Type type)
        {
            var alreadyYield = new HashSet<PropertyInfo>();

            foreach (PropertyInfo propertyInfo in type.GetRuntimeProperties().Where(IsPubliclyAccessible))
            {
                alreadyYield.Add(propertyInfo);
                yield return propertyInfo;
            }

            MethodInfo[] implementedMethods = GetImplementedInterfaceMethods(type).ToArray();

            for (Type currentType = type; currentType != null; currentType = currentType.GetTypeInfo().BaseType)
            {
                foreach (PropertyInfo propertyInfo in currentType.GetRuntimeProperties())
                {
                    if (alreadyYield.Contains(propertyInfo))
                        continue;

                    bool IsExplicitMethod(MethodBase m) => m != null && m.IsPrivate && m.IsFinal && implementedMethods.Any(x => m.Name == x.Name);

                    if (IsExplicitMethod(propertyInfo.GetMethod) || IsExplicitMethod(propertyInfo.SetMethod))
                    {
                        alreadyYield.Add(propertyInfo);
                        yield return propertyInfo;
                    }
                }
            }
        }

        static public bool IsPubliclyAccessible(this FieldInfo fieldInfo)
        {
            return fieldInfo.IsPublic && !fieldInfo.IsInitOnly;
        }

        static public bool IsPubliclyAccessible(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetMethod != null && IsPubliclyAccessible(propertyInfo.GetMethod)
                || propertyInfo.SetMethod != null && IsPubliclyAccessible(propertyInfo.SetMethod);
        }

        static public bool IsPubliclyAccessible(this MethodBase methodInfo)
        {
            return methodInfo.IsPublic && !methodInfo.IsAbstract && !methodInfo.IsStatic;
        }

        static public IEnumerable<MethodInfo> GetImplementedInterfaceMethods(this Type type)
        {
            TypeInfo typeInfo = type.GetTypeInfo();
            return typeInfo.ImplementedInterfaces
                .Select(implementedInterface => typeInfo.GetRuntimeInterfaceMap(implementedInterface))
                .SelectMany(interfaceMapping => interfaceMapping.TargetMethods);
        }
    }
}