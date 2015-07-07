using System;
using System.Linq;
using System.Reflection;

namespace Diese.Injection.Factories.Data
{
    internal class ConstructorData
    {
        public ConstructorInfo ConstructorInfo { get; private set; }
        public ParameterData[] ParametersData { get; private set; }

        public int Count
        {
            get { return ParametersData.Length; }
        }

        public ConstructorData(ConstructorInfo constructorInfo)
        {
            ConstructorInfo = constructorInfo;

            ParameterInfo[] parameterInfos = constructorInfo.GetParameters();
            ParametersData = new ParameterData[parameterInfos.Length];

            for (int i = 0; i < ParametersData.Length; i++)
            {
                ParameterInfo parameterInfo = parameterInfos[i];
                var parameter = new ParameterData {Type = parameterInfo.ParameterType};

                Attribute[] attributes = parameterInfo.GetCustomAttributes(typeof(ServiceKeyAttribute)).ToArray();
                if (attributes.Length > 0)
                    parameter.ServiceKey = ((ServiceKeyAttribute)attributes.First()).Key;

                ParametersData[i] = parameter;
            }
        }
    }
}