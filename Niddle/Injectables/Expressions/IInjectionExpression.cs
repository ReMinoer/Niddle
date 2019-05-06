using System;
using System.Linq.Expressions;

namespace Niddle.Injectables.Expressions
{
    public interface IInjectionExpression
    {
        Expression BuildInjectionExpression(Expression targetExpression, Expression valueExpression, Type memberType);
    }
}