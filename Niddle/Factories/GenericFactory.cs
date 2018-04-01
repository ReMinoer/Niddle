using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Niddle.Factories.Base;
using Niddle.Utils;

namespace Niddle.Factories
{
    internal class GenericFactory : GenericFactoryBase
    {
        private readonly Dictionary<Type, IDependencyFactory> _dependencyFactories;
        private readonly int _constructorIndex;
        public override InstanceOrigin? InstanceOrigin { get; }
        public ConstructorInfo Constructor { get; }

        public GenericFactory(Type genericTypeDescription, InstanceOrigin instanceOrigin, object serviceKey, ConstructorInfo constructor, Substitution substitution)
            : base(genericTypeDescription, serviceKey, substitution)
        {
            InstanceOrigin = instanceOrigin;
            Constructor = constructor;

            _dependencyFactories = new Dictionary<Type, IDependencyFactory>();
            _constructorIndex = Type.GetTypeInfo().DeclaredConstructors.IndexOf(constructor);
        }

        public override IDependencyFactory GetFactory(Type[] genericTypeArguments)
        {
            Type derivedType = Type.MakeGenericType(genericTypeArguments);

            if (_dependencyFactories.ContainsKey(derivedType))
                return _dependencyFactories[derivedType];

            ConstructorInfo derivedConstructor = derivedType.GetTypeInfo().DeclaredConstructors.ElementAt(_constructorIndex);

            IDependencyFactory factory;
            switch (InstanceOrigin)
            {
                case Niddle.InstanceOrigin.Instantiation:
                    factory = new NewInstanceFactory(derivedType, null, derivedConstructor, Substitution.Forbidden);
                    break;
                case Niddle.InstanceOrigin.Registration:
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