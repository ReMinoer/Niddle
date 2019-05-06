using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Niddle.Injectables.Base;

namespace Niddle.Injectables
{
    public class InjectableMethod<TTarget, TArguments, TReject> : InjectableMethodBase<TTarget, TArguments, TReject, MethodInfo>
        where TArguments : IEnumerable
    {
        public InjectableMethod(MethodInfo methodInfo)
            : base(methodInfo)
        {
        }

        protected override Expression BuildCall(MethodInfo methodBase, ParameterExpression target, IEnumerable<Expression> parameters)
        {
            return methodBase.IsStatic
                ? Expression.Call(methodBase, parameters)
                : Expression.Call(target, methodBase, parameters);
        }
    }
}