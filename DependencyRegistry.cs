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
        private readonly Dictionary<Type, IDependencyFactory> _defaultFactories;
        private readonly Dictionary<KeyedService, IDependencyFactory> _keyedFactories;

        public DependencyRegistry()
        {
            _defaultFactories = new Dictionary<Type, IDependencyFactory>();
            _keyedFactories = new Dictionary<KeyedService, IDependencyFactory>();
        }

        public IDependencyFactory this[Type type, object serviceKey = null]
        {
            get
            {
                IDependencyFactory factory;

                if (serviceKey != null)
                {
                    if (!_keyedFactories.TryGetValue(new KeyedService(type, serviceKey), out factory))
                        throw new NotRegisterException(serviceKey);

                    return factory;
                }

                if (!_defaultFactories.TryGetValue(type, out factory))
                    throw new NotRegisterException(type);

                return factory;
            }
        }

        public void RegisterInstance<TAbstract>(object instance, object serviceKey = null)
        {
            RegisterInstance(typeof(TAbstract), instance, serviceKey);
        }

        public void RegisterInstance(Type abstractType, object instance, object serviceKey = null)
        {
            AddFactory(new InstanceFactory(abstractType, instance, serviceKey));
        }

        public void Register<T>(Subsistence subsistence = Subsistence.Transient, object serviceKey = null,
            ConstructorInfo constructor = null)
        {
            Register(typeof(T), subsistence, serviceKey, constructor);
        }

        public void Register(Type type, Subsistence subsistence = Subsistence.Transient, object serviceKey = null,
            ConstructorInfo constructor = null)
        {
            Register(type, type, subsistence, serviceKey, constructor);
        }

        public void Register<TAbstract, TImplmentation>(Subsistence subsistence = Subsistence.Transient,
            object serviceKey = null, ConstructorInfo constructor = null) where TImplmentation : TAbstract
        {
            Register(typeof(TAbstract), typeof(TImplmentation), subsistence, serviceKey, constructor);
        }

        public void Register<TAbstract>(Type implementationType, Subsistence subsistence = Subsistence.Transient,
            object serviceKey = null, ConstructorInfo constructor = null)
        {
            Register(typeof(TAbstract), implementationType, subsistence, serviceKey, constructor);
        }

        public void Register(Type abstractType, Type implementationType, Subsistence subsistence = Subsistence.Transient,
            object serviceKey = null, ConstructorInfo constructor = null)
        {
            if (subsistence == Subsistence.Singleton)
                AddFactory(new SingletonFactory(abstractType, serviceKey,
                    constructor ?? GetDefaultConstructor(implementationType)));
            else
                AddFactory(new TransientFactory(abstractType, serviceKey,
                    constructor ?? GetDefaultConstructor(implementationType)));
        }

        private void AddFactory(IDependencyFactory factory)
        {
            if (factory.ServiceKey != null)
                AddToKeyedFactory(factory);
            else
                AddToDefaultFactory(factory);
        }

        private void AddToDefaultFactory(IDependencyFactory factory)
        {
            Type type = factory.Type;

            if (type == null)
                throw new NullReferenceException("Registered type is null !");

            if (_defaultFactories.ContainsKey(type))
                throw new AlreadyRegisterException(type);

            _defaultFactories.Add(type, factory);
        }

        private void AddToKeyedFactory(IDependencyFactory factory)
        {
            var keyedService = new KeyedService(factory.Type, factory.ServiceKey);

            if (_keyedFactories.ContainsKey(keyedService))
                throw new AlreadyRegisterException(factory.Type, factory.ServiceKey);

            _keyedFactories.Add(keyedService, factory);
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
                return _type.GetHashCode() + _serviceKey.GetHashCode();
            }
        }
    }
}