using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Niddle.Injectables.Base;

namespace Niddle.Injectables
{
    public class InjectableConstructor<TTarget, TArguments, TReject> : InjectableMethodBase<TTarget, TArguments, TReject, ConstructorInfo>
        where TArguments : IEnumerable
    {
        public InjectableConstructor(ConstructorInfo constructorInfo)
            : base(constructorInfo)
        {
        }

        protected override Expression BuildCall(ConstructorInfo methodBase, ParameterExpression target, IEnumerable<Expression> parameters)
        {
            return Expression.New(methodBase, parameters);
        }
    }
}