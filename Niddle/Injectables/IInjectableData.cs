using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Niddle.Injectables
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Method | AttributeTargets.Constructor)]
    public class ResolvableAttribute : Attribute
    {
        public object Key { get; set; }

        public virtual IInjectionScenario GetInjectionScenario(Type injectableType) => new AssignementInjection();

        public virtual IResolvableInjectable<TTarget, object> GetResolvableInjectable<TTarget>(IInjectable<TTarget, object> injectable)
        {
            return new ResolvableInjectable<TTarget>(injectable) { Key = Key };
        }

        public virtual IResolvableRejecter<TTarget, object, object> GetResolvableRejecter<TTarget>(IRejecter<TTarget, object, object> rejecter)
        {
            return new ResolvableRejecter<TTarget, object>(rejecter) { Key = Key };
        }
    }

    public class ResolvableInjectable<TTarget> : IResolvableInjectable<TTarget, object>
    {
        private readonly IInjectable<TTarget, object> _injectable;

        public object Key { get; set; }
        public Type Type => _injectable.Type;
        
        public ResolvableInjectable(IInjectable<TTarget, object> injectable)
        {
            _injectable = injectable;
        }

        public object Resolve(IDependencyInjector injector) => injector.Resolve(_injectable.Type, serviceKey: Key);
        public bool TryResolve(IDependencyInjector injector, out object value) => injector.TryResolve(out value, _injectable.Type, serviceKey: Key);
        
        public void Inject(TTarget target, object value) => _injectable.Inject(target, value);
        public void ResolveAndInject(IDependencyInjector injector, TTarget target) => _injectable.Inject(target, Resolve(injector));
    }

    public class ResolvableRejecter<TTarget, TRejecter> : ResolvableInjectable<TTarget>, IResolvableRejecter<TTarget, object, TRejecter>
    {
        private readonly IRejecter<TTarget, object, TRejecter> _rejecter;

        public ResolvableRejecter(IRejecter<TTarget, object, TRejecter> rejecter)
            : base(rejecter)
        {
            _rejecter = rejecter;
        }

        public TRejecter Reject(TTarget target, object value) => _rejecter.Reject(target, value);
        public TRejecter ResolveAndReject(IDependencyInjector injector, TTarget target) => _rejecter.Reject(target, Resolve(injector));
    }

    public class PopulatableAttribute : ResolvableAttribute
    {
        private readonly string _populateMethodName;
        private readonly Type _parameterType;

        public PopulatableAttribute(string populateMethodName = nameof(ICollection<object>.Add), Type parameterType = null)
        {
            _populateMethodName = populateMethodName;
            _parameterType = parameterType;
        }

        public override IInjectionScenario GetInjectionScenario(Type injectableType) => new PopulateInjection(injectableType.GetPopulateMethod(_populateMethodName, _parameterType));

        public override IResolvableInjectable<TTarget, object> GetResolvableInjectable<TTarget>(IInjectable<TTarget, object> injectable)
        {
            return new PopulatableInjectable<TTarget>(injectable, _populateMethodName) { Key = Key };
        }

        public override IResolvableRejecter<TTarget, object, object> GetResolvableRejecter<TTarget>(IRejecter<TTarget, object, object> rejecter)
        {
            return new PopulatableRejecter<TTarget>(rejecter, _populateMethodName) { Key = Key };
        }
    }

    public class PopulatableInjectable<TTarget> : IResolvableInjectable<TTarget, object>
    {
        private readonly string _populateMethodName;
        private readonly IInjectable<TTarget, object> _injectable;

        public object Key { get; set; }
        public Type Type => _injectable.Type;

        public PopulatableInjectable(IInjectable<TTarget, object> injectable, string populateMethodName = nameof(ICollection<object>.Add))
        {
            _injectable = injectable;
            _populateMethodName = populateMethodName;
        }

        public object Resolve(IDependencyInjector injector) => injector.ResolveMany(_injectable.Type, serviceKey: Key);
        public bool TryResolve(IDependencyInjector injector, out object value)
        {
            var valueList = new List<object>();
            foreach (Type injectableType in _injectable.Type.GetPopulatableTypes(_populateMethodName))
            {
                if (injector.TryResolveMany(out IEnumerable resolvedValues, injectableType, serviceKey: Key))
                    valueList.AddRange(resolvedValues.Cast<object>());
            }

            value = valueList;
            return valueList.Count > 0;
        }

        bool IResolvable.TryResolve(IDependencyInjector injector, out object value) => TryResolve(injector, out value);
    
        public void Inject(TTarget target, object value) => _injectable.Inject(target, value);
        public void ResolveAndInject(IDependencyInjector injector, TTarget target) => _injectable.Inject(target, Resolve(injector));
    }

    static public class PopulatableInjectionUtils
    {
        static public IEnumerable<Type> GetPopulatableTypes(this Type memberType, string populateMethodName)
        {
            return GetPopulateMethods(memberType, populateMethodName).Select(x => x.ParameterType());
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
            return GetPopulateMethods(type, populateMethodName, itemTypeInfo).Aggregate((x,y) => x.ParameterTypeInfo().IsAssignableFrom(y.ParameterTypeInfo()) ? y : x);
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

    public class PopulatableRejecter<TTarget> : PopulatableInjectable<TTarget>, IResolvableRejecter<TTarget, object, object>
    {
        private readonly IRejecter<TTarget, object, object> _rejecter;

        public PopulatableRejecter(IRejecter<TTarget, object, object> rejecter, string populateMethodName = nameof(ICollection<object>.Add))
            : base(rejecter, populateMethodName)
        {
            _rejecter = rejecter;
        }

        public object Reject(TTarget target, object value) => _rejecter.Reject(target, value);
        public object ResolveAndReject(IDependencyInjector injector, TTarget target) => _rejecter.Reject(target, Resolve(injector));
    }

    public interface IInjectionScenario
    {
        Expression BuildInjectionExpression(Expression targetExpression, Expression valueExpression);
    }

    public class AssignementInjection : IInjectionScenario
    {
        public Expression BuildInjectionExpression(Expression targetExpression, Expression valueExpression)
        {
            return Expression.Assign(targetExpression, valueExpression);
        }
    }

    public class PopulateInjection : IInjectionScenario
    {
        public MethodInfo PopulateMethodInfo { get; }

        public PopulateInjection(MethodInfo populateMethodInfo)
        {
            PopulateMethodInfo = populateMethodInfo;
        }

        public Expression BuildInjectionExpression(Expression targetExpression, Expression valueExpression)
        {
            ParameterExpression enumerator = Expression.Variable(typeof(IEnumerator));
            MethodCallExpression getEnumerator = Expression.Call(valueExpression, InjectionMethodInfos.GetEnumeratorMethodInfo);
            BinaryExpression assignEnumerator = Expression.Assign(enumerator, getEnumerator);

            MethodCallExpression moveNext = Expression.Call(enumerator, InjectionMethodInfos.EnumeratorMoveNextMethodInfo);
            MethodCallExpression current = Expression.Call(enumerator, InjectionMethodInfos.EnumeratorCurrentMethodInfo);
            MethodCallExpression populate = Expression.Call(targetExpression, PopulateMethodInfo, current);
            LabelTarget loopBreakTarget = Expression.Label();
            GotoExpression loopBreak = Expression.Break(loopBreakTarget);
            LoopExpression populateWhileMoveNext = Expression.Loop(Expression.IfThenElse(Expression.IsTrue(moveNext), populate, loopBreak), loopBreakTarget);

            return Expression.Block(assignEnumerator, populateWhileMoveNext);
        }
    }

    public interface IInjectable
    {
        Type Type { get; }
    }

    public interface IInjectable<in TTarget, in TValue> : IInjectable
    {
        void Inject(TTarget target, TValue value);
    }

    public interface IRejecter<in TTarget, in TValue, out TReject> : IInjectable<TTarget, TValue>
    {
        TReject Reject(TTarget target, TValue value);
    }

    public interface IResolvable
    {
        object Key { get; }
        object Resolve(IDependencyInjector injector);
        bool TryResolve(IDependencyInjector injector, out object value);
    }

    public interface IResolvable<TValue> : IResolvable
    {
        new TValue Resolve(IDependencyInjector injector);
        bool TryResolve(IDependencyInjector injector, out TValue value);
    }

    public interface IResolvableInjectable<in TTarget, TValue> : IResolvable<TValue>, IInjectable<TTarget, TValue>
    {
        void ResolveAndInject(IDependencyInjector injector, TTarget target);
    }

    public interface IResolvableRejecter<in TTarget, TValue, out TReject> : IResolvableInjectable<TTarget, TValue>, IRejecter<TTarget, TValue, TReject>
    {
        TReject ResolveAndReject(IDependencyInjector injector, TTarget target);
    }

    public class InjectableBase : IInjectable
    {
        public Type Type { get; }
        public IInjectionScenario InjectionScenario { get; }

        protected InjectableBase(IInjectionScenario injectionScenario, Type type)
        {
            InjectionScenario = injectionScenario;
            Type = type;
        }
    }

    public abstract class InjectableTargetMemberBase<TTarget, TValue, TMemberInfo> : InjectableBase, IInjectable<TTarget, TValue>
        where TMemberInfo : MemberInfo
    {
        private readonly Action<TTarget, TValue> _injectionDelegate;

        protected InjectableTargetMemberBase(IInjectionScenario injectionScenario, TMemberInfo memberInfo, Type memberType)
            : base(injectionScenario, memberType)
        {
            _injectionDelegate = BuildInjectionDelegate(memberInfo);
        }

        protected abstract MemberExpression GetTargetMemberExpression(TMemberInfo memberInfo, ParameterExpression target);

        private Action<TTarget, TValue> BuildInjectionDelegate(TMemberInfo memberInfo)
        {
            ParameterExpression target = Expression.Parameter(typeof(TTarget));
            ParameterExpression value = Expression.Parameter(typeof(TValue));

            MemberExpression targetMember = GetTargetMemberExpression(memberInfo, target);
            Expression body = InjectionScenario.BuildInjectionExpression(targetMember, value);

            return (Action<TTarget, TValue>)Expression.Lambda(body, target, value).Compile();
        }

        public void Inject(TTarget target, TValue value)
        {
            _injectionDelegate(target, value);
        }
    }

    public class InjectableField<TTarget, TValue> : InjectableTargetMemberBase<TTarget, TValue, FieldInfo>
    {
        public InjectableField(IInjectionScenario injectionScenario, FieldInfo fieldInfo)
            : base(injectionScenario, fieldInfo, fieldInfo.FieldType)
        {
        }

        protected override MemberExpression GetTargetMemberExpression(FieldInfo memberInfo, ParameterExpression target)
        {
            return Expression.Field(target, memberInfo);
        }
    }

    public class InjectableProperty<TTarget, TValue> : InjectableTargetMemberBase<TTarget, TValue, PropertyInfo>
    {
        public InjectableProperty(IInjectionScenario injectionScenario, PropertyInfo propertyInfo)
            : base(injectionScenario, propertyInfo, propertyInfo.PropertyType)
        {
        }

        protected override MemberExpression GetTargetMemberExpression(PropertyInfo memberInfo, ParameterExpression target)
        {
            return Expression.Property(target, memberInfo);
        }
    }

    public class InjectableParameter : InjectableBase
    {
        public bool HasDefaultValue { get; }
        public object DefaultValue { get; }

        public InjectableParameter(IInjectionScenario injectionScenario, ParameterInfo parameterInfo)
            : base(injectionScenario, parameterInfo.ParameterType)
        {
            HasDefaultValue = parameterInfo.HasDefaultValue;
            if (HasDefaultValue)
                DefaultValue = parameterInfo.DefaultValue;
        }
    }

    static public class InjectionMethodInfos
    {
        static public readonly MethodInfo GetEnumeratorMethodInfo;
        static public readonly MethodInfo EnumeratorMoveNextMethodInfo;
        static public readonly MethodInfo EnumeratorCurrentMethodInfo;
        static public readonly MethodInfo GetNextMethodInfo;
        
        static InjectionMethodInfos()
        {
            GetEnumeratorMethodInfo = typeof(IEnumerable).GetTypeInfo().GetDeclaredMethod(nameof(IEnumerable.GetEnumerator));
            EnumeratorMoveNextMethodInfo = typeof(IEnumerator).GetTypeInfo().GetDeclaredMethod(nameof(IEnumerator.MoveNext));
            EnumeratorCurrentMethodInfo = typeof(IEnumerator).GetTypeInfo().GetDeclaredMethod(nameof(IEnumerator.Current));
            GetNextMethodInfo = typeof(InjectionMethodInfos).GetTypeInfo().GetDeclaredMethod(nameof(GetNext));
        }

        static private object GetNext(IEnumerator enumerator)
        {
            enumerator.MoveNext();
            return enumerator.Current;
        }
    }

    public abstract class InjectableMethodBase<TTarget, TArguments, TReject, TMethod> : InjectableBase, IRejecter<TTarget, TArguments, TReject>
        where TArguments : IEnumerable
        where TMethod : MethodBase
    {
        public InjectableParameter[] Parameters { get; }
        protected Func<TTarget, TArguments, TReject> Delegate { get; }

        protected InjectableMethodBase(IInjectionScenario injectionScenario, TMethod methodBase)
            : base(injectionScenario, typeof(TArguments))
        {
            Parameters = methodBase.GetParameters().Select(x => new InjectableParameter(x.GetCustomAttribute<ResolvableAttribute>()?.GetInjectionScenario(x.ParameterType) ?? new AssignementInjection(), x)).ToArray();
            Delegate = BuildDelegate(methodBase, Parameters);
        }

        protected abstract Expression BuildCall(TMethod methodBase, ParameterExpression target, IEnumerable<Expression> parameters);

        private Func<TTarget, TArguments, TReject> BuildDelegate(TMethod methodBase, InjectableParameter[] injectableParameters)
        {
            ParameterExpression target = Expression.Parameter(typeof(TTarget));
            ParameterExpression arguments = Expression.Parameter(typeof(TArguments));
            var bodyExpressions = new List<Expression>();

            MethodCallExpression getEnumerator = Expression.Call(arguments, InjectionMethodInfos.GetEnumeratorMethodInfo);
            ParameterExpression enumerator = Expression.Variable(typeof(IEnumerator));
            BinaryExpression assignEnumerator = Expression.Assign(enumerator, getEnumerator);
            bodyExpressions.Add(assignEnumerator);

            var methodParameters = new List<Expression>();
            foreach (InjectableParameter injectableParameter in injectableParameters)
            {
                Type parameterType = injectableParameter.Type;

                MethodCallExpression getNext = Expression.Call(InjectionMethodInfos.GetNextMethodInfo, enumerator);
                UnaryExpression castedParameterValue = Expression.Convert(getNext, parameterType);

                ParameterExpression parameterVariable = Expression.Variable(parameterType);
                Expression parameterInjection = injectableParameter.InjectionScenario.BuildInjectionExpression(parameterVariable, castedParameterValue);

                methodParameters.Add(parameterVariable);
                bodyExpressions.Add(parameterInjection);
            }

            Expression call = BuildCall(methodBase, target, methodParameters);
            bodyExpressions.Add(call);

            BlockExpression body = Expression.Block(bodyExpressions);

            return (Func<TTarget, TArguments, TReject>)Expression.Lambda(body, target, arguments).Compile();
        }

        public void Inject(TTarget target, TArguments value)
        {
            Delegate(target, value);
        }

        public TReject Reject(TTarget target, TArguments value)
        {
            return Delegate(target, value);
        }
    }

    public class InjectableConstructor<TTarget, TArguments, TReject> : InjectableMethodBase<TTarget, TArguments, TReject, ConstructorInfo>
        where TArguments : IEnumerable
    {
        public InjectableConstructor(IInjectionScenario injectionScenario, ConstructorInfo constructorInfo)
            : base(injectionScenario, constructorInfo)
        {
        }

        protected override Expression BuildCall(ConstructorInfo methodBase, ParameterExpression target, IEnumerable<Expression> parameters)
        {
            return Expression.New(methodBase, parameters);
        }
    }

    public class InjectableMethod<TTarget, TArguments, TReject> : InjectableMethodBase<TTarget, TArguments, TReject, MethodInfo>
        where TArguments : IEnumerable
    {
        public InjectableMethod(IInjectionScenario injectionScenario, MethodInfo methodInfo)
            : base(injectionScenario, methodInfo)
        {
        }

        protected override Expression BuildCall(MethodInfo methodBase, ParameterExpression target, IEnumerable<Expression> parameters)
        {
            return Expression.Call(target, methodBase, parameters);
        }
    }

    public class ResolvableMembersProvider<T>
    {
        public IReadOnlyCollection<IResolvableInjectable<T, object>> Injectables { get; }

        public ResolvableMembersProvider()
        {
            Type type = typeof(T);

            var injectables = new List<IResolvableInjectable<T, object>>();
            Injectables = new ReadOnlyCollection<IResolvableInjectable<T, object>>(injectables);

            foreach (FieldInfo fieldInfo in type.GetRuntimeFields())
            {
                if (!fieldInfo.IsPublic)
                    continue;

                var attribute = fieldInfo.GetCustomAttribute<ResolvableAttribute>();
                if (attribute == null)
                    continue;

                injectables.Add(attribute.GetResolvableInjectable(new InjectableField<T, object>(attribute.GetInjectionScenario(fieldInfo.FieldType), fieldInfo)));
            }

            foreach (PropertyInfo propertyInfo in type.GetRuntimeProperties())
            {
                if (propertyInfo.SetMethod == null || !propertyInfo.SetMethod.IsPublic
                    && propertyInfo.GetMethod == null || !propertyInfo.GetMethod.IsPublic)
                    continue;

                var attribute = propertyInfo.GetCustomAttribute<ResolvableAttribute>();
                if (attribute == null)
                    continue;

                injectables.Add(attribute.GetResolvableInjectable(new InjectableProperty<T, object>(attribute.GetInjectionScenario(propertyInfo.PropertyType), propertyInfo)));
            }
        }
    }
}