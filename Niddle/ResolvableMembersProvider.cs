using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Niddle.Attributes.Base;

namespace Niddle
{
    public interface IResolvableMembersProvider<in TTarget>
    {
        IEnumerable<IResolvableInjectable<TTarget, object, object>> ForType<T>();
        IEnumerable<IResolvableInjectable<TTarget, object, object>> ForType(Type type);
    }

    public class ResolvableMembersProvider : ResolvableMembersProvider<object> {}
    public class ResolvableMembersProvider<TTarget> : IResolvableMembersProvider<TTarget>
    {
        public IEnumerable<IResolvableInjectable<TTarget, object, object>> ForType<T>() => ForType(typeof(T));
        public IEnumerable<IResolvableInjectable<TTarget, object, object>> ForType(Type type)
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

            //foreach (MethodInfo methodInfo in type.GetRuntimeMethods())
            //{
            //    if (!methodInfo.IsPublic)
            //        continue;

            //    var attribute = methodInfo.GetCustomAttribute<ResolvableAttributeBase>();
            //    if (attribute == null)
            //        continue;

            //    yield return methodInfo.AsResolvableRejecter<TTarget>();
            //}
        }
    }
    
    public class ResolvableMembersCache : ResolvableMembersCache<object>
    {
        public ResolvableMembersCache(IResolvableMembersProvider<object> provider)
            :base(provider)
        {
        }
    }

    public class ResolvableMembersCache<TTarget> : IResolvableMembersProvider<TTarget>
    {
        private readonly IResolvableMembersProvider<TTarget> _provider;
        private readonly Dictionary<Type, IResolvableInjectable<TTarget, object, object>[]> _cache;

        public ResolvableMembersCache(IResolvableMembersProvider<TTarget> provider)
        {
            _provider = provider;
            _cache = new Dictionary<Type, IResolvableInjectable<TTarget, object, object>[]>();
        }

        public IEnumerable<IResolvableInjectable<TTarget, object, object>> ForType<T>() => ForType(typeof(T));
        public IEnumerable<IResolvableInjectable<TTarget, object, object>> ForType(Type type)
        {
            if (!_cache.TryGetValue(type, out IResolvableInjectable<TTarget, object, object>[] resolvableMembers))
                _cache[type] = resolvableMembers = _provider.ForType(type).ToArray();

            return resolvableMembers;
        }
    }
}