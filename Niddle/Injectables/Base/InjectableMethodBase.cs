using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Niddle.Attributes.Base;
using Niddle.Injectables.Expressions;
using Niddle.Utils;

namespace Niddle.Injectables.Base
{
    public abstract class InjectableMethodBase<TTarget, TArguments, TReject, TMethod> : IRejecterMethod<TTarget, TArguments, TReject>
        where TArguments : IEnumerable
        where TMethod : MethodBase
    {
        public Type Type { get; } = typeof(TArguments);
        public IReadOnlyCollection<InjectableParameter> Parameters { get; }
        protected Func<TTarget, TArguments, TReject> Delegate { get; }

        protected InjectableMethodBase(TMethod methodBase)
        {
            InjectableParameter[] parameters = methodBase.GetParameters().Select(x => new InjectableParameter(CustomAttributeExtensions.GetCustomAttribute<ResolvableAttributeBase>((ParameterInfo)x)?.GetInjectionScenario(x.ParameterType) ?? new AssignementInjection(), x)).ToArray();
            Parameters = new ReadOnlyCollection<InjectableParameter>(parameters);

            Delegate = BuildDelegate(methodBase, Parameters);
        }

        protected abstract Expression BuildCall(TMethod methodBase, ParameterExpression target, IEnumerable<Expression> parameters);

        private Func<TTarget, TArguments, TReject> BuildDelegate(TMethod methodBase, IEnumerable<InjectableParameter> injectableParameters)
        {
            ParameterExpression target = Expression.Parameter(typeof(TTarget));
            ParameterExpression arguments = Expression.Parameter(typeof(TArguments));

            var bodyVariables = new List<ParameterExpression>();
            var bodyExpressions = new List<Expression>();

            ParameterExpression enumerator = Expression.Variable(typeof(IEnumerator));
            bodyVariables.Add(enumerator);
            
            MethodCallExpression getEnumerator = Expression.Call(arguments, InjectionReflectionInfos.GetEnumeratorMethodInfo);
            BinaryExpression assignEnumerator = Expression.Assign(enumerator, getEnumerator);
            bodyExpressions.Add(assignEnumerator);

            var methodParameters = new List<Expression>();
            foreach (InjectableParameter injectableParameter in injectableParameters)
            {
                Type parameterType = injectableParameter.Type;

                MethodCallExpression getNext = Expression.Call(InjectionReflectionInfos.GetNextMethodInfo, enumerator);
                UnaryExpression castedParameterValue = Expression.Convert(getNext, parameterType);

                ParameterExpression parameterVariable = Expression.Variable(parameterType);
                bodyVariables.Add(parameterVariable);
                methodParameters.Add(parameterVariable);

                Expression parameterInjection = injectableParameter.InjectionExpression.BuildInjectionExpression(parameterVariable, castedParameterValue, parameterType);
                bodyExpressions.Add(parameterInjection);
            }

            Expression call = BuildCall(methodBase, target, methodParameters);
            Expression castedCall = Expression.Convert(call, typeof(TReject));
            bodyExpressions.Add(castedCall);

            BlockExpression body = Expression.Block(bodyVariables, bodyExpressions);

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
}