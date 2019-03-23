using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Niddle.Factories.Data
{
    public class ConstructorData
    {
        public Delegate Delegate { get; }
        public ParameterData[] ParametersData { get; private set; }

        public ConstructorData(ConstructorInfo constructorInfo)
        {
            NewExpression newExpression = Expression.New(constructorInfo, constructorInfo.GetParameters().Select(x => Expression.Parameter(x.ParameterType, x.Name)));
            Delegate = Expression.Lambda(newExpression, newExpression.Arguments.Cast<ParameterExpression>()).Compile();
            
            ParametersData = GetParametersData(constructorInfo);
        }

        public ConstructorData(MethodInfo methodInfo)
        {
            MethodCallExpression methodCallExpression = Expression.Call(methodInfo);
            Delegate = Expression.Lambda(methodCallExpression, methodCallExpression.Arguments.Cast<ParameterExpression>()).Compile();

            ParametersData = GetParametersData(methodInfo);
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