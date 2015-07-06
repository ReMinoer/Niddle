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
        private DependencyInjector _injector;
        private readonly Dictionary<Type, IDependencyFactory> _defaultFactories;
        private readonly Dictionary<object, IDependencyFactory> _keyedFactories;

        public DependencyInjector Injector
        {
            set { _injector = value; }
        }

        public DependencyRegistry()
        {
            _defaultFactories = new Dictionary<Type, IDependencyFactory>();
            _keyedFactories = new Dictionary<object, IDependencyFactory>();
        }

        public object this[Type type, object serviceKey]
        {
            get
            {
                IDependencyFactory factory;

                if (serviceKey != null)
                {
                    if (!_keyedFactories.TryGetValue(serviceKey, out factory))
                        throw new NotRegisterException(serviceKey);

                    return factory.Get(_injector);
                }

                if (!_defaultFactories.TryGetValue(type, out factory))
                    throw new NotRegisterException(type);
                
                return factory.Get(_injector);
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

        public void Register<T>(Subsistence subsistence = Subsistence.Transient, object serviceKey = null, ConstructorInfo constructor = null)
        {
            Register(typeof(T), subsistence, serviceKey, constructor);
        }

        public void Register(Type type, Subsistence subsistence = Subsistence.Transient, object serviceKey = null, ConstructorInfo constructor = null)
        {
            Register(type, type, subsistence, serviceKey, constructor);
        }

        public void Register<TAbstract, TImplmentation>(Subsistence subsistence = Subsistence.Transient, object serviceKey = null,
            ConstructorInfo constructor = null) where TImplmentation : TAbstract
        {
            Register(typeof(TAbstract), typeof(TImplmentation), subsistence, serviceKey, constructor);
        }

        public void Register<TAbstract>(Type implementationType, Subsistence subsistence = Subsistence.Transient, object serviceKey = null,
            ConstructorInfo constructor = null)
        {
            Register(typeof(TAbstract), implementationType, subsistence, serviceKey, constructor);
        }

        public void Register(Type abstractType, Type implementationType, Subsistence subsistence = Subsistence.Transient, object serviceKey = null,
            ConstructorInfo constructor = null)
        {
            if (subsistence == Subsistence.Singleton)
                AddFactory(new SingletonFactory(abstractType, serviceKey, constructor ?? GetDefaultConstructor(implementationType)));
            else
                AddFactory(new TransientFactory(abstractType, serviceKey, constructor ?? GetDefaultConstructor(implementationType)));
        }

        private void AddFactory(IDependencyFactory factory)
        {
            if (factory.ServiceKey != null)
                AddKeyedFactory(factory);
            else
                AddDefaultFactory(factory);
        }

        private void AddDefaultFactory(IDependencyFactory factory)
        {
            if (factory.Type == null)
                throw new NullReferenceException("Registered type is null !");

            if (_defaultFactories.ContainsKey(factory.Type))
                throw new AlreadyRegisterException(factory.Type);

            _defaultFactories.Add(factory.Type, factory);
        }

        private void AddKeyedFactory(IDependencyFactory factory)
        {
            if (_keyedFactories.ContainsKey(factory.ServiceKey))
                throw new AlreadyRegisterException(factory.ServiceKey);

            _keyedFactories.Add(factory.ServiceKey, factory);
        }

        private static ConstructorInfo GetDefaultConstructor(Type type)
        {
            return type.GetConstructors()
                   .Aggregate((min, next) => next.GetParameters().Length < min.GetParameters().Length ? next : min);
        }
    }
}