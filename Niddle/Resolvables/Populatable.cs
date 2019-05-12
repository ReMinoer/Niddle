using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Niddle.Resolvables.Base;
using Niddle.Utils;

namespace Niddle.Resolvables
{
    public class Populatable : SingleResolvableBase<IEnumerable>
    {
        public string PopulateMethodName { get; set; }
        public Type Type { get; set; }

        public override IEnumerable Resolve(IDependencyResolver resolver)
        {
            foreach (Type itemType in Type.GetPopulatableTypes(PopulateMethodName))
                foreach (object item in resolver.ResolveMany(itemType, Key, InstanceOrigins, resolver, AdditionalArguments))
                    yield return item;
        }

        public override IOptional<IEnumerable> TryResolve(IDependencyResolver resolver)
        {
            var valueList = new List<object>();
            foreach (Type itemType in Type.GetPopulatableTypes(PopulateMethodName))
            {
                if (!resolver.TryResolveMany(out IEnumerable resolvedValues, itemType, Key, InstanceOrigins, resolver, AdditionalArguments))
                    continue;

                foreach (object item in resolvedValues)
                    valueList.Add(item);
            }

            return valueList.Count > 0 ? valueList : Optional<IEnumerable>.NoValue;
        }
    }

    static public class PopulatableInjectionUtils
    {
        static public IEnumerable<Type> GetPopulatableTypes(this Type memberType, string populateMethodName)
        {
            return GetPopulateMethods(memberType, populateMethodName).Select(x => ParameterType(x));
        }

        static private IEnumerable<MethodInfo> GetPopulateMethods(this Type type, string populateMethodName)
        {
            return type.GetRuntimeMethods().Where(x => x.IsPopulateMethod(populateMethodName));
        }

        static private IEnumerable<MethodInfo> GetPopulateMethods(this Type type, string populateMethodName, TypeInfo itemType)
        {
            return type.GetPopulateMethods(populateMethodName).Where(x => x.ParameterTypeInfo().IsAssignableFrom(itemType));
        }

        static public MethodInfo GetPopulateMethod(this Type type, string populateMethodName, Type parameterType = null)
        {
            if (parameterType == null)
                return type.GetPopulateMethods(populateMethodName).First();

            return type.GetPopulateMethods(populateMethodName).First(x => x.ParameterType() == parameterType);
        }

        static public MethodInfo GetBestPopulateMethod(this Type type, string populateMethodName, Type itemType)
        {
            TypeInfo itemTypeInfo = itemType.GetTypeInfo();
            return GetPopulateMethods(type, populateMethodName, itemTypeInfo).Aggregate((x, y) => x.ParameterTypeInfo().IsAssignableFrom(y.ParameterTypeInfo()) ? y : x);
        }

        static private bool IsPopulateMethod(this MethodBase methodBase, string populateMethodName)
        {
            return methodBase.Name == populateMethodName && methodBase.GetParameters().Length == 1;
        }

        static private Type ParameterType(this MethodBase methodBase)
        {
            return methodBase.GetParameters()[0].ParameterType;
        }

        static private TypeInfo ParameterTypeInfo(this MethodBase methodBase)
        {
            return methodBase.ParameterType().GetTypeInfo();
        }
    }
}