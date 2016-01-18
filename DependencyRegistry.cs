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

        public IDependencyFactory this[Type type, object serviceKey = null]
        {
            get
            {
                IDependencyFactory factory;

                if (TryGetFactory(out factory, type, serviceKey))
                    return factory;

                throw new NotRegisterException(type, serviceKey);
            }
        }

        public bool TryGetFactory(out IDependencyFactory factory, Type type, object serviceKey)
        {
            factory = _dependencyFactories[type, serviceKey];

            if (factory == null && type.IsGenericType)
            {
                IGenericFactory genericFactory = _genericFactories[type.GetGenericTypeDefinition(), serviceKey];
                if (genericFactory != null)
                    factory = genericFactory.GetFactory(type.GenericTypeArguments);
            }

            return factory != null;
        }

        public void RegisterInstance<TAbstract>(object instance, object serviceKey = null, Substitution substitution = Substitution.Forbidden)
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

        public void RegisterGeneric(Type genericTypeDescription, Subsistence subsistence = Subsistence.Transient, object serviceKey = null,
            ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden)
        {
            RegisterGeneric(genericTypeDescription, genericTypeDescription, subsistence, serviceKey, constructor, substitution);
        }

        public void RegisterGeneric(Type abstractTypeDescription, Type genericTypeDescription, Subsistence subsistence = Subsistence.Transient, object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden)
        {
            AddGenericFactory(new GenericFactory(abstractTypeDescription, subsistence, serviceKey,
                constructor ?? GetDefaultConstructor(genericTypeDescription), substitution));
        }

        public void Register<T>(Subsistence subsistence = Subsistence.Transient, object serviceKey = null,
            ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden)
        {
            Register(typeof(T), subsistence, serviceKey, constructor, substitution);
        }

        public void Register(Type type, Subsistence subsistence = Subsistence.Transient, object serviceKey = null,
            ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden)
        {
            Register(type, type, subsistence, serviceKey, constructor, substitution);
        }

        public void Register<TAbstract, TImplementation>(Subsistence subsistence = Subsistence.Transient,
            object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden)
            where TImplementation : TAbstract
        {
            Register(typeof(TAbstract), typeof(TImplementation), subsistence, serviceKey, constructor, substitution);
        }

        public void Register(Type abstractType, Type implementationType, Subsistence subsistence = Subsistence.Transient,
            object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden)
        {
            if (subsistence == Subsistence.Singleton)
                AddDependencyFactory(new SingletonFactory(abstractType, serviceKey,
                    constructor ?? GetDefaultConstructor(implementationType), substitution));
            else
                AddDependencyFactory(new TransientFactory(abstractType, serviceKey,
                    constructor ?? GetDefaultConstructor(implementationType), substitution));
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

        private struct KeyedService
        {
            private readonly Type _type;
            private readonly object _serviceKey;

            public KeyedService(Type type, object serviceKey)
            {
                _type = type;
                _serviceKey = serviceKey;
            }

            public override int GetHashCode()
            {
                return _type.GetHashCode() ^ _serviceKey.GetHashCode();
            }
        }

        private sealed class KeyableServiceRegistry<TValue>
            where TValue : class, IInjectionService
        {
            private readonly Dictionary<Type, TValue> _defaultFactories;
            private readonly Dictionary<KeyedService, TValue> _keyedFactories;

            public KeyableServiceRegistry()
            {
                _defaultFactories = new Dictionary<Type, TValue>();
                _keyedFactories = new Dictionary<KeyedService, TValue>();
            }

            public TValue this[Type type, object serviceKey = null]
            {
                get
                {
                    TValue factory;

                    if (serviceKey != null)
                    {
                        if (_keyedFactories.TryGetValue(new KeyedService(type, serviceKey), out factory))
                            return factory;

                        return null;
                    }

                    if (_defaultFactories.TryGetValue(type, out factory))
                        return factory;

                    return null;
                }
            }

            public void AddToDefaultFactories(TValue factory)
            {
                Type type = factory.Type;

                if (type == null)
                    throw new NullReferenceException("Registered type is null !");

                if (_defaultFactories.ContainsKey(type))
                {
                    if (_defaultFactories[type].Substitution == Substitution.Forbidden)
                        throw new AlreadyRegisterException(type);

                    _defaultFactories.Remove(type);
                }

                _defaultFactories.Add(type, factory);
            }

            public void AddToKeyedFactories(TValue factory)
            {
                Type type = factory.Type;
                object serviceKey = factory.ServiceKey;
                var keyedService = new KeyedService(type, serviceKey);

                if (_keyedFactories.ContainsKey(keyedService))
                {
                    if (_keyedFactories[keyedService].Substitution == Substitution.Forbidden)
                        throw new AlreadyRegisterException(serviceKey);

                    _keyedFactories.Remove(keyedService);
                }

                _keyedFactories.Add(keyedService, factory);
            }
        }
    }
}