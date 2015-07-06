using System;
using System.Collections.Generic;
using System.Reflection;
using Diese.Injection.Exceptions;
using Diese.Injection.Factories.Data;

namespace Diese.Injection.Factories
{
    internal class TransientFactory : IDependencyFactory
    {
        static private readonly Stack<IDependencyFactory> FactoryStack = new Stack<IDependencyFactory>();

        private readonly ConstructorData _constructorData;

        public Type Type { get; private set; }
        public object ServiceKey { get; private set; }

        private bool _alreadyInvoke;

        public TransientFactory(Type type, object serviceKey, ConstructorInfo constructorInfo)
        {
            Type = type;
            ServiceKey = serviceKey;

            _constructorData = new ConstructorData(constructorInfo);
        }

        public virtual object Get(IDependencyInjector injector)
        {
            FactoryStack.Push(this);

            if (_alreadyInvoke)
                throw new CyclicDependencyException(FactoryStack);

            var parameters = new object[_constructorData.Count];
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterData data = _constructorData.ParametersData[i];
                parameters[i] = injector.Resolve(data.Type, data.ServiceKey);
            }

            _alreadyInvoke = true;
            object instance = _constructorData.ConstructorInfo.Invoke(parameters);
            _alreadyInvoke = false;

            FactoryStack.Pop();

            return instance;
        }
    }
}