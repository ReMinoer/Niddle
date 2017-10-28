using System;
using System.Collections.Generic;
using System.Reflection;
using Diese.Injection.Factories.Base;

namespace Diese.Injection.Factories
{
    internal class GenericFactory : GenericFactoryBase
    {
        private readonly Dictionary<Type, IDependencyFactory> _dependencyFactories;
        private readonly int _constructorIndex;
        public override InstanceOrigin InstanceOrigin { get; }
        public ConstructorInfo Constructor { get; }

        public GenericFactory(Type genericTypeDescription, InstanceOrigin instanceOrigin, object serviceKey, ConstructorInfo constructor, Substitution substitution)
            : base(genericTypeDescription, serviceKey, substitution)
        {
            InstanceOrigin = instanceOrigin;
            Constructor = constructor;

            _dependencyFactories = new Dictionary<Type, IDependencyFactory>();

            ConstructorInfo[] allConstructors = Type.GetConstructors();
            for (int i = 0; i < allConstructors.Length; i++)
                if (allConstructors[i] == constructor)
                {
                    _constructorIndex = i;
                    break;
                }
        }

        public override IDependencyFactory GetFactory(Type[] genericTypeArguments)
        {
            Type derivedType = Type.MakeGenericType(genericTypeArguments);

            if (_dependencyFactories.ContainsKey(derivedType))
                return _dependencyFactories[derivedType];

            ConstructorInfo derivedConstructor = derivedType.GetConstructors()[_constructorIndex];

            IDependencyFactory factory;
            switch (InstanceOrigin)
            {
                case InstanceOrigin.Instantiation:
                    factory = new NewInstanceFactory(derivedType, null, derivedConstructor, Substitution.Forbidden);
                    break;
                case InstanceOrigin.Registration:
                    factory = new SingletonFactory(derivedType, null, derivedConstructor, Substitution.Forbidden);
                    break;
                default:
                    throw new ArgumentException();
            }

            _dependencyFactories.Add(derivedType, factory);
            return factory;
        }
    }
}