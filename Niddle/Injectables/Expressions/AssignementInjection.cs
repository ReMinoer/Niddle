using System;
using System.Linq.Expressions;

namespace Niddle.Injectables.Expressions
{
    public class AssignementInjection : IInjectionExpression
    {
        public Expression BuildInjectionExpression(Expression targetExpression, Expression valueExpression, Type memberType)
        {
            UnaryExpression castedValue = Expression.Convert(valueExpression, memberType);
            return Expression.Assign(targetExpression, castedValue);
        }
    }
}