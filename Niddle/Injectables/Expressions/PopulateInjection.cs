using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Niddle.Utils;

namespace Niddle.Injectables.Expressions
{
    public class PopulateInjection : IInjectionExpression
    {
        public MethodInfo PopulateMethodInfo { get; }

        public PopulateInjection(MethodInfo populateMethodInfo)
        {
            PopulateMethodInfo = populateMethodInfo;
        }

        public Expression BuildInjectionExpression(Expression targetExpression, Expression valueExpression, Type memberType)
        {
            UnaryExpression castedValue = Expression.Convert(valueExpression, typeof(IEnumerable));
            MethodCallExpression getEnumerator = Expression.Call(castedValue, InjectionReflectionInfos.GetEnumeratorMethodInfo);

            ParameterExpression enumerator = Expression.Variable(typeof(IEnumerator));
            BinaryExpression assignEnumerator = Expression.Assign(enumerator, getEnumerator);

            MethodCallExpression moveNext = Expression.Call(enumerator, InjectionReflectionInfos.EnumeratorMoveNextMethodInfo);
            MemberExpression current = Expression.Property(enumerator, InjectionReflectionInfos.EnumeratorCurrentMethodInfo);
            UnaryExpression castedCurrent = Expression.Convert(current, PopulateMethodInfo.GetParameters()[0].ParameterType);

            MethodCallExpression populate = Expression.Call(targetExpression, PopulateMethodInfo, castedCurrent);
            LabelTarget loopBreakTarget = Expression.Label();
            GotoExpression loopBreak = Expression.Break(loopBreakTarget);
            LoopExpression populateWhileMoveNext = Expression.Loop(Expression.IfThenElse(Expression.IsTrue(moveNext), populate, loopBreak), loopBreakTarget);

            return Expression.Block(new []{ enumerator }, assignEnumerator, populateWhileMoveNext);
        }
    }
}