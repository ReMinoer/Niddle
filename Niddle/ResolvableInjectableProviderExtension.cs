using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Niddle.Attributes;
using Niddle.Attributes.Base;
using Niddle.Hybrids;
using Niddle.Injectables;
using Niddle.Injectables.Expressions;
using Niddle.Resolvables;

namespace Niddle
{
    static public class ResolvableInjectableProviderExtension
    {
        // Attributes

        static private readonly ResolvableAttributeBase DefaultResolvableAttribute = new ResolvableAttribute();

        static private Attribute[] Attributes(this MemberInfo memberInfo)
        {
            return memberInfo.GetCustomAttributes().ToArray();
        }

        static private Attribute[] Attributes(this ParameterInfo parameterInfo)
        {
            return parameterInfo.GetCustomAttributes().ToArray();
        }

        static private ResolvableAttributeBase ResolvableAttribute(this IEnumerable<Attribute> attributes)
        {
            return attributes.OfType<ResolvableAttributeBase>().FirstOrDefault() ?? DefaultResolvableAttribute;
        }

        // AsResolvable
        
        static public IResolvable AsResolvable(this PropertyInfo propertyInfo) => AsResolvableBase(propertyInfo, propertyInfo.PropertyType);
        static public IResolvable AsResolvable(this FieldInfo fieldInfo) => AsResolvableBase(fieldInfo, fieldInfo.FieldType);
        static private IResolvable AsResolvableBase(MemberInfo memberInfo, Type memberType)
        {
            Attribute[] attributes = memberInfo.Attributes();
            return attributes.ResolvableAttribute().GetResolvable(memberType, attributes);
        }

        static public IResolvable AsResolvable(this ParameterInfo parameterInfo)
        {
            Attribute[] attributes = parameterInfo.Attributes();
            IResolvable resolvable = attributes.ResolvableAttribute().GetResolvable(parameterInfo.ParameterType, attributes);

            if (parameterInfo.HasDefaultValue)
                resolvable = new DefaultableResolvable(resolvable, parameterInfo.DefaultValue);

            return resolvable;
        }

        static public EnumerableResolvable AsResolvable(this MethodBase methodBase)
        {
            return new EnumerableResolvable(methodBase.GetParameters().Select(x => x.AsResolvable()));
        }

        // AsInjectable

        static public InjectableProperty<object, object> AsInjectable(this PropertyInfo propertyInfo)
            => AsInjectableBase(propertyInfo, propertyInfo.PropertyType, x => new InjectableProperty<object, object>(x, propertyInfo));

        static public InjectableField<object, object> AsInjectable(this FieldInfo fieldInfo)
            => AsInjectableBase(fieldInfo, fieldInfo.FieldType, x => new InjectableField<object, object>(x, fieldInfo));

        static private TInjectable AsInjectableBase<TInjectable>(MemberInfo memberInfo, Type memberType, Func<IInjectionExpression, TInjectable> injectableFactory)
        {
            return injectableFactory(memberInfo.Attributes().ResolvableAttribute().GetInjectionScenario(memberType));
        }

        static public InjectableParameter AsInjectable(this ParameterInfo parameterInfo)
        {
            IInjectionExpression injectionExpression = parameterInfo.Attributes().ResolvableAttribute().GetInjectionScenario(parameterInfo.ParameterType);
            return new InjectableParameter(injectionExpression, parameterInfo);
        }

        static public InjectableConstructor<object, IEnumerable, object> AsInjectable(this ConstructorInfo constructorInfo)
            => new InjectableConstructor<object, IEnumerable, object>(constructorInfo);

        static public InjectableMethod<object, IEnumerable, object> AsInjectable(this MethodInfo methodInfo)
            => new InjectableMethod<object, IEnumerable, object>(methodInfo);

        // AsResolvableInjectable

        static public ResolvableInjectable<IResolvable<object>, InjectableProperty<TTarget, object>, TTarget, object, object> AsResolvableInjectable<TTarget>(this PropertyInfo propertyInfo)
            => AsResolvableInjectableBase<InjectableProperty<TTarget, object>, TTarget, object, object>(propertyInfo, propertyInfo.PropertyType, x => new InjectableProperty<TTarget, object>(x, propertyInfo));

        static public ResolvableInjectable<IResolvable<object>, InjectableField<TTarget, object>, TTarget, object, object> AsResolvableInjectable<TTarget>(this FieldInfo fieldInfo)
            => AsResolvableInjectableBase<InjectableField<TTarget, object>, TTarget, object, object>(fieldInfo, fieldInfo.FieldType, x => new InjectableField<TTarget, object>(x, fieldInfo));

        static private ResolvableInjectable<IResolvable<TResolvableValue>, TInjectable, TTarget, TResolvableValue, TInjectableValue> AsResolvableInjectableBase<TInjectable, TTarget, TResolvableValue, TInjectableValue>(MemberInfo memberInfo, Type defaultType, Func<IInjectionExpression, TInjectable> injectableFactory)
            where TInjectable : IInjectable<TTarget, TInjectableValue>
            where TResolvableValue : TInjectableValue
        {
            Attribute[] attributes = memberInfo.Attributes();
            ResolvableAttributeBase resolvableAttribute = attributes.ResolvableAttribute();

            IResolvable<TResolvableValue> resolvable = resolvableAttribute.GetResolvable<TResolvableValue>(defaultType, attributes);
            TInjectable injectable = injectableFactory(resolvableAttribute.GetInjectionScenario(defaultType));

            return new ResolvableInjectable<IResolvable<TResolvableValue>, TInjectable, TTarget, TResolvableValue, TInjectableValue>(resolvable, injectable);
        }

        static public ResolvableInjectable<IResolvable, InjectableParameter> AsResolvableInjectable(this ParameterInfo parameterInfo)
        {
            Attribute[] attributes = parameterInfo.Attributes();
            ResolvableAttributeBase resolvableAttribute = attributes.ResolvableAttribute();
            
            var injectable = new InjectableParameter(resolvableAttribute.GetInjectionScenario(parameterInfo.ParameterType), parameterInfo);

            IResolvable resolvable = resolvableAttribute.GetResolvable(injectable.Type, attributes);
            if (injectable.HasDefaultValue)
                resolvable = new DefaultableResolvable(resolvable, injectable.DefaultValue);

            return new ResolvableInjectable<IResolvable, InjectableParameter>(resolvable, injectable);
        }

        static public ResolvableRejecter<IResolvable<IEnumerable>, InjectableConstructor<TTarget, IEnumerable, object>, TTarget, IEnumerable, IEnumerable, object> AsResolvableRejecter<TTarget>(this ConstructorInfo constructorInfo)
            => AsResolvableMethodRejecterBase<InjectableConstructor<TTarget, IEnumerable, object>, TTarget, object>(constructorInfo, new InjectableConstructor<TTarget, IEnumerable, object>(constructorInfo));

        static public ResolvableRejecter<IResolvable<IEnumerable>, InjectableMethod<TTarget, IEnumerable, object>, TTarget, IEnumerable, IEnumerable, object> AsResolvableRejecter<TTarget>(this MethodInfo methodInfo)
            => AsResolvableMethodRejecterBase<InjectableMethod<TTarget, IEnumerable, object>, TTarget, object>(methodInfo, new InjectableMethod<TTarget, IEnumerable, object>(methodInfo));

        static private ResolvableRejecter<IResolvable<IEnumerable>, TRejecter, TTarget, IEnumerable, IEnumerable, TReject> AsResolvableMethodRejecterBase<TRejecter, TTarget, TReject>(MethodBase methodBase, TRejecter rejecter)
            where TRejecter : IRejecterMethod<TTarget, IEnumerable, TReject>
        {
            var resolvable = new EnumerableResolvable(methodBase.GetParameters().Select(x => x.AsResolvableInjectable()));
            return new ResolvableRejecter<IResolvable<IEnumerable>, TRejecter, TTarget, IEnumerable, IEnumerable, TReject>(resolvable, rejecter);
        }

        static public ResolvableInjectable<IResolvable<object>, InjectableProperty<object, object>, object, object, object> AsResolvableInjectable(this PropertyInfo propertyInfo) => propertyInfo.AsResolvableInjectable<object>();
        static public ResolvableInjectable<IResolvable<object>, InjectableField<object, object>, object, object, object> AsResolvableInjectable(this FieldInfo fieldInfo) => fieldInfo.AsResolvableInjectable<object>();
        static public ResolvableRejecter<IResolvable<IEnumerable>, InjectableConstructor<object, IEnumerable, object>, object, IEnumerable, IEnumerable, object> AsResolvableRejecter(this ConstructorInfo constructorInfo) => constructorInfo.AsResolvableRejecter<object>();
        static public ResolvableRejecter<IResolvable<IEnumerable>, InjectableMethod<object, IEnumerable, object>, object, IEnumerable, IEnumerable, object> AsResolvableRejecter(this MethodInfo methodInfo) => methodInfo.AsResolvableRejecter<object>();
    }
}