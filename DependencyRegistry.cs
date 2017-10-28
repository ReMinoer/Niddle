using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Diese.Injection.Exceptions;
using Diese.Injection.Factories;

namespace Diese.Injection
{
    public class DependencyRegistry : IDependencyRegistry
    {
        private readonly KeyableServiceRegistry<IDependencyFactory> _dependencyFactories;
        private readonly KeyableServiceRegistry<IGenericFactory> _genericFactories;

        public DependencyRegistry()
        {
            _dependencyFactories = new KeyableServiceRegistry<IDependencyFactory>();
            _genericFactories = new KeyableServiceRegistry<IGenericFactory>();
        }

        public IDependencyFactory this[Type type, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All]
        {
            get
            {
                if (TryGetFactory(out IDependencyFactory factory, type, serviceKey, instanceOrigins))
                    return factory;

                throw new NotRegisterException(type, serviceKey);
            }
        }

        public bool TryGetFactory(out IDependencyFactory factory, Type type, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All)
        {
            factory = _dependencyFactories[type, serviceKey, instanceOrigins];

            if (factory == null && type.IsGenericType)
            {
                IGenericFactory genericFactory = _genericFactories[type.GetGenericTypeDefinition(), serviceKey, instanceOrigins];
                if (genericFactory != null)
                    factory = genericFactory.GetFactory(type.GenericTypeArguments);
            }

            return factory != null;
        }

        public void Register<TAbstract, TImplementation>(object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden)
            where TImplementation : TAbstract
            => Register(typeof(TAbstract), typeof(TImplementation), serviceKey, constructor, substitution);

        public void Register(Type abstractType, Type implementationType, object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden)
            => Register(abstractType, serviceKey, constructor ?? GetDefaultConstructor(implementationType), substitution);

        public void Register<T>(object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden)
            => Register(typeof(T), serviceKey, constructor, substitution);

        public void Register(Type type, object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden)
        {
            AddDependencyFactory(new NewInstanceFactory(type, serviceKey, constructor ?? GetDefaultConstructor(type), substitution));
        }

        public void RegisterSingleton<TAbstract, TImplementation>(object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden)
            where TImplementation : TAbstract
            => RegisterSingleton(typeof(TAbstract), typeof(TImplementation), serviceKey, constructor, substitution);

        public void RegisterSingleton(Type abstractType, Type implementationType, object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden)
            => RegisterSingleton(abstractType, serviceKey, constructor ?? GetDefaultConstructor(implementationType), substitution);

        public void RegisterSingleton<T>(object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden)
            => RegisterSingleton(typeof(T), serviceKey, constructor, substitution);

        public void RegisterSingleton(Type type, object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden)
        {
            AddDependencyFactory(new SingletonFactory(type, serviceKey, constructor ?? GetDefaultConstructor(type), substitution));
        }

        public void RegisterGeneric(Type genericTypeDescription, object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden)
            => RegisterGeneric(genericTypeDescription, genericTypeDescription, serviceKey, constructor, substitution);

        public void RegisterGeneric(Type abstractTypeDescription, Type genericTypeDescription, object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden)
        {
            AddGenericFactory(new GenericFactory(abstractTypeDescription, InstanceOrigin.Instantiation, serviceKey, constructor ?? GetDefaultConstructor(genericTypeDescription), substitution));
        }

        public void RegisterGenericSingleton(Type genericTypeDescription, object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden)
            => RegisterGenericSingleton(genericTypeDescription, genericTypeDescription, serviceKey, constructor, substitution);

        public void RegisterGenericSingleton(Type abstractTypeDescription, Type genericTypeDescription, object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden)
        {
            AddGenericFactory(new GenericFactory(abstractTypeDescription, InstanceOrigin.Registration, serviceKey, constructor ?? GetDefaultConstructor(genericTypeDescription), substitution));
        }

        public void RegisterInstance<TAbstract>(TAbstract instance, object serviceKey = null, Substitution substitution = Substitution.Forbidden)
        {
            RegisterInstance(typeof(TAbstract), instance, serviceKey, substitution);
        }

        public void RegisterInstance(Type abstractType, object instance, object serviceKey = null, Substitution substitution = Substitution.Forbidden)
        {
            AddDependencyFactory(new InstanceFactory(abstractType, instance, serviceKey, substitution));
        }

        public void RegisterLazy<T>(Func<T> factory, object serviceKey = null, Substitution substitution = Substitution.Forbidden)
        {
            AddDependencyFactory(new LazyFactory<T>(factory, serviceKey, substitution));
        }

        public void RegisterAction<TIn>(Action<TIn> action, object serviceKey = null, Substitution substitution = Substitution.Forbidden)
        {
            AddDependencyFactory(new ActionFactory<TIn>(action, serviceKey, substitution));
        }

        public void RegisterFunc<TOut>(Func<TOut> func, object serviceKey = null, Substitution substitution = Substitution.Forbidden)
        {
            AddDependencyFactory(new FuncFactory<TOut>(func, serviceKey, substitution));
        }

        public void RegisterFunc<TIn, TOut>(Func<TIn, TOut> func, object serviceKey = null, Substitution substitution = Substitution.Forbidden)
        {
            AddDependencyFactory(new FuncFactory<TIn, TOut>(func, serviceKey, substitution));
        }

        public void Link<TLinked, TRegistered>(object registeredKey = null, object serviceKey = null,
            Substitution substitution = Substitution.Forbidden)
            where TRegistered : TLinked
        {
            Link(typeof(TLinked), typeof(TRegistered), registeredKey, serviceKey, substitution);
        }

        public void Link(Type linkedType, Type registeredType, object registeredKey = null, object serviceKey = null,
            Substitution substitution = Substitution.Forbidden)
        {
            AddDependencyFactory(new LinkedFactory(linkedType, this[registeredType, registeredKey], serviceKey, substitution));
        }

        public void LinkGeneric(Type linkedTypeDescription, Type registeredTypeDescription, object registeredKey = null, object serviceKey = null, Substitution substitution = Substitution.Forbidden)
        {
            AddGenericFactory(new LinkedGenericFactory(linkedTypeDescription, _genericFactories[registeredTypeDescription, registeredKey], serviceKey, substitution));
        }

        private void AddDependencyFactory(IDependencyFactory factory)
        {
            if (factory.ServiceKey != null)
                _dependencyFactories.AddToKeyedFactories(factory);
            else
                _dependencyFactories.AddToDefaultFactories(factory);
        }

        private void AddGenericFactory(IGenericFactory genericFactory)
        {
            if (genericFactory.ServiceKey != null)
                _genericFactories.AddToKeyedFactories(genericFactory);
            else
                _genericFactories.AddToDefaultFactories(genericFactory);
        }

        static private ConstructorInfo GetDefaultConstructor(Type type)
        {
            return type.GetConstructors()
                .Aggregate((min, next) => next.GetParameters().Length < min.GetParameters().Length ? next : min);
        }

        private class OriginFactories<TFactory>
            where TFactory : class, IInjectionService
        {
            private TFactory _instantiationFactory;
            private TFactory _registrationFactory;

            public TFactory this[InstanceOrigin instanceOrigin]
            {
                get
                {
                    switch (instanceOrigin)
                    {
                        case InstanceOrigin.Instantiation:
                            return _instantiationFactory;
                        case InstanceOrigin.Registration:
                            return _registrationFactory;
                        default:
                            throw new NotSupportedException();
                    }
                }
                set
                {
                    switch (instanceOrigin)
                    {
                        case InstanceOrigin.Instantiation:
                            _instantiationFactory = value;
                            break;
                        case InstanceOrigin.Registration:
                            _registrationFactory = value;
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }
            }

            public TFactory this[InstanceOrigins instanceOrigins]
            {
                get
                {
                    if ((instanceOrigins & InstanceOrigins.Registration) != 0 && _registrationFactory != null)
                        return _registrationFactory;
                    if ((instanceOrigins & InstanceOrigins.Instantiation) != 0)
                        return _instantiationFactory;

                    return null;
                }
            }
        }

        private sealed class KeyableServiceRegistry<TFactory>
            where TFactory : class, IInjectionService
        {
            private readonly Dictionary<Type, OriginFactories<TFactory>> _defaultFactories;
            private readonly Dictionary<object, Dictionary<Type, OriginFactories<TFactory>>> _keyedFactories;

            public KeyableServiceRegistry()
            {
                _defaultFactories = new Dictionary<Type, OriginFactories<TFactory>>();
                _keyedFactories = new Dictionary<object, Dictionary<Type, OriginFactories<TFactory>>>();
            }

            public TFactory this[Type type, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All]
            {
                get
                {
                    OriginFactories<TFactory> factories;
                    
                    if (serviceKey != null)
                    {
                        if (_keyedFactories.TryGetValue(serviceKey, out Dictionary<Type, OriginFactories<TFactory>> factoryDictionary)
                            && factoryDictionary.TryGetValue(type, out factories))
                            return factories[instanceOrigins];

                        return null;
                    }

                    if (_defaultFactories.TryGetValue(type, out factories))
                        return factories[instanceOrigins];

                    return null;
                }
            }

            public void AddToDefaultFactories(TFactory factory)
            {
                Type type = factory.Type;

                if (type == null)
                    throw new ArgumentException("Registered type is null !");
                
                if (_defaultFactories.TryGetValue(type, out OriginFactories<TFactory> factoryBySubsistence))
                {
                    if (factoryBySubsistence[factory.InstanceOrigin].Substitution == Substitution.Forbidden)
                        throw new AlreadyRegisterException(type);
                }
                else
                {
                    factoryBySubsistence = new OriginFactories<TFactory>();
                    _defaultFactories[type] = factoryBySubsistence;
                }

                factoryBySubsistence[factory.InstanceOrigin] = factory;
            }

            public void AddToKeyedFactories(TFactory factory)
            {
                Type type = factory.Type;
                object serviceKey = factory.ServiceKey;

                OriginFactories<TFactory> originFactories;
                if (_keyedFactories.TryGetValue(serviceKey, out Dictionary<Type, OriginFactories<TFactory>> factoryDictionary))
                {
                    if (factoryDictionary.TryGetValue(type, out originFactories))
                    {
                        if (originFactories[factory.InstanceOrigin].Substitution == Substitution.Forbidden)
                            throw new AlreadyRegisterException(serviceKey);
                    }
                    else
                    {
                        originFactories = new OriginFactories<TFactory>();
                        factoryDictionary[type] = originFactories;
                    }
                }
                else
                {
                    factoryDictionary = new Dictionary<Type, OriginFactories<TFactory>>();
                    _keyedFactories.Add(serviceKey, factoryDictionary);
                    
                    originFactories = new OriginFactories<TFactory>();
                    factoryDictionary[type] = originFactories;
                }

                originFactories[factory.InstanceOrigin] = factory;
            }
        }
    }
}