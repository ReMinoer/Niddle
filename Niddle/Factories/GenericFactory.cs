using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Niddle.Factories.Base;
using Niddle.Utils;

namespace Niddle.Factories
{
    public class GenericFactory : GenericFactoryBase
    {
        private readonly Type _genericTypeDefinition;
        private readonly Dictionary<Type, IDependencyFactory> _dependencyFactories;
        private readonly int _constructorIndex;
        public override InstanceOrigin? InstanceOrigin { get; }

        public GenericFactory(Type abstractTypeDefinition, Type genericTypeDefinition, InstanceOrigin instanceOrigin, object serviceKey, ConstructorInfo constructor, Substitution substitution)
            : base(abstractTypeDefinition, serviceKey, substitution)
        {
            _genericTypeDefinition = genericTypeDefinition ?? abstractTypeDefinition;
            InstanceOrigin = instanceOrigin;

            _dependencyFactories = new Dictionary<Type, IDependencyFactory>();
            _constructorIndex = _genericTypeDefinition.GetTypeInfo().DeclaredConstructors.Where(x => x.IsPublic).IndexOf(constructor);
        }

        public override IDependencyFactory GetFactory(IEnumerable<Type> genericTypeArguments)
        {
            Type derivedType = _genericTypeDefinition.MakeGenericType(genericTypeArguments as Type[] ?? genericTypeArguments.ToArray());

            if (_dependencyFactories.ContainsKey(derivedType))
                return _dependencyFactories[derivedType];

            ConstructorInfo derivedConstructor = derivedType.GetTypeInfo().DeclaredConstructors.Where(x => x.IsPublic).ElementAt(_constructorIndex);
            IResolvableRejecter<object, IEnumerable, object> resolvableDerivedConstructor = derivedConstructor.AsResolvableRejecter();

            IDependencyFactory factory;
            switch (InstanceOrigin)
            {
                case Niddle.InstanceOrigin.Instantiation:
                    factory = new NewInstanceFactory(derivedType, null, resolvableDerivedConstructor, Substitution.Forbidden);
                    break;
                case Niddle.InstanceOrigin.Registration:
                    factory = new SingletonFactory(derivedType, null, resolvableDerivedConstructor, Substitution.Forbidden);
                    break;
                default:
                    throw new ArgumentException();
            }

            _dependencyFactories.Add(derivedType, factory);
            return factory;
        }
    }
}