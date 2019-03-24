﻿using System;
using System.Reflection;
using Niddle.Base;

namespace Niddle.Factories.Data
{
    internal class MemberData
    {
        public Type Type { get; private set; }
        public object ServiceKey { get; private set; }
        public InjectableAttributeBase InjectableAttribute { get; private set; }

        public MemberData(Type type, MemberInfo memberInfo)
        {
            Type = type;
            
            InjectableAttribute = memberInfo.GetCustomAttribute<InjectableAttributeBase>();
            ServiceKey = InjectableAttribute?.Key;
        }
    }
}