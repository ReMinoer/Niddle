using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Niddle.Factories.Data
{
    public class ConstructorData
    {
        public Func<object[], object> Delegate { get; }
        public ParameterData[] ParametersData { get; private set; }

        public ConstructorData(ConstructorInfo constructorInfo)
        {
            Delegate = BuildLambda(constructorInfo, arguments => Expression.New(constructorInfo, arguments));
            ParametersData = GetParametersData(constructorInfo);
        }

        public ConstructorData(MethodInfo methodInfo)
        {
            Delegate = BuildLambda(methodInfo, arguments => Expression.Call(methodInfo, arguments));
            ParametersData = GetParametersData(methodInfo);
        }

        static private Func<object[], object> BuildLambda(MethodBase methodBase, Func<IEnumerable<Expression>, Expression> bodyBuilder)
        {
            Type[] types = methodBase.GetParameters().Select(x => x.ParameterType).ToArray();
            
            ParameterExpression array = Expression.Parameter(typeof(object[]));
            IEnumerable<Expression> parametersFromArray = types.Select((t, i) => Expression.Convert(Expression.ArrayAccess(array, Expression.Constant(i, typeof(int))), t));

            Expression body = Expression.Convert(bodyBuilder(parametersFromArray), typeof(object));

            return (Func<object[], object>)Expression.Lambda(body, array).Compile();
        }

        static public ParameterData[] GetParametersData(MethodBase methodBase)
        {
            ParameterInfo[] parameterInfos = methodBase.GetParameters();

            var parametersData = new ParameterData[parameterInfos.Length];
            for (int i = 0; i < parametersData.Length; i++)
                parametersData[i] = new ParameterData(parameterInfos[i]);

            return parametersData;
        }
    }
}