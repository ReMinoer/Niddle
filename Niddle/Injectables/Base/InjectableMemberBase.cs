using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Niddle.Injectables.Expressions;

namespace Niddle.Injectables.Base
{
    public abstract class InjectableMemberBase<TTarget, TValue, TMemberInfo> : InjectableBase, IInjectable<TTarget, TValue>
        where TMemberInfo : MemberInfo
    {
        private readonly Action<TTarget, TValue> _injectionDelegate;

        protected InjectableMemberBase(IInjectionExpression injectionExpression, TMemberInfo memberInfo, Type memberType, Delegate targetGetterDelegate)
            : base(injectionExpression, memberType)
        {
            _injectionDelegate = BuildInjectionDelegate(memberInfo, memberType, targetGetterDelegate);
        }

        protected abstract MemberExpression GetTargetMemberExpression(TMemberInfo memberInfo, Expression target);

        private Action<TTarget, TValue> BuildInjectionDelegate(TMemberInfo memberInfo, Type memberType, Delegate targetGetterDelegate)
        {
            ParameterExpression target = Expression.Parameter(typeof(TTarget));
            ParameterExpression value = Expression.Parameter(typeof(TValue));
            
            Expression selectedTarget;
            if (targetGetterDelegate != null)
            {
                MethodInfo targetGetterMethodInfo = targetGetterDelegate.GetMethodInfo();
                ParameterInfo[] parameterInfos = targetGetterMethodInfo.GetParameters();

                UnaryExpression castedTarget = Expression.Convert(target, parameterInfos[parameterInfos.Length - 1].ParameterType);
                selectedTarget = Expression.Invoke(Expression.Constant(targetGetterDelegate), castedTarget);
            }
            else
                selectedTarget = Expression.Convert(target, memberInfo.DeclaringType);

            MemberExpression targetMember = GetTargetMemberExpression(memberInfo, selectedTarget);
            Expression injection = InjectionExpression.BuildInjectionExpression(targetMember, value, memberType);

            Expression body = Expression.Block(injection, Expression.Empty());

            return (Action<TTarget, TValue>)Expression.Lambda(body, target, value).Compile();
        }

        public void Inject(TTarget target, TValue value)
        {
            _injectionDelegate(target, value);
        }
    }
}