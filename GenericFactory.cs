using System;
using System.Collections.Generic;
using System.Reflection;
using Diese.Injection.Factories;

namespace Diese.Injection
{
    internal class GenericFactory : ServiceFactoryBase, IGenericFactory
    {
        private readonly Dictionary<Type, IDependencyFactory> _dependencyFactories;
        private readonly int _constructorIndex;
        public Subsistence Subsistence { get; private set; }
        public ConstructorInfo Constructor { get; private set; }

        public GenericFactory(Type genericTypeDescription, Subsistence subsistence, object serviceKey, ConstructorInfo constructor, Substitution substitution)
            : base(genericTypeDescription, serviceKey, substitution)
        {
            Subsistence = subsistence;
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

        public IDependencyFactory GetFactory(Type[] genericTypeArguments)
        {
            Type derivedType = Type.MakeGenericType(genericTypeArguments);

            if (_dependencyFactories.ContainsKey(derivedType))
                return _dependencyFactories[derivedType];

            ConstructorInfo derivedConstructor = derivedType.GetConstructors()[_constructorIndex];

            IDependencyFactory factory = Subsistence == Subsistence.Singleton
                ? new SingletonFactory(derivedType, null, derivedConstructor, Substitution.Forbidden)
                : new TransientFactory(derivedType, null, derivedConstructor, Substitution.Forbidden);

            _dependencyFactories.Add(derivedType, factory);
            return factory;
        }
    }
}