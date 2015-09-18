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
                        throw new NotRegisterException(type, serviceKey);

                    return factory;
                }

                if (!_defaultFactories.TryGetValue(type, out factory))
                    throw new NotRegisterException(type);

                return factory;
            }
        }

        public void RegisterInstance<TAbstract>(object instance, object serviceKey = null, Substitution substitution = Substitution.Forbidden)
        {
            RegisterInstance(typeof(TAbstract), instance, serviceKey, substitution);
        }

        public void RegisterInstance(Type abstractType, object instance, object serviceKey = null, Substitution substitution = Substitution.Forbidden)
        {
            AddFactory(new InstanceFactory(abstractType, instance, serviceKey, substitution));
        }

        public void RegisterLazy<T>(Func<T> factory, object serviceKey = null, Substitution substitution = Substitution.Forbidden)
        {
            AddFactory(new LazyFactory<T>(factory, serviceKey, substitution));
        }

        public void RegisterAction<TIn>(Action<TIn> action, object serviceKey = null, Substitution substitution = Substitution.Forbidden)
        {
            AddFactory(new ActionFactory<TIn>(action, serviceKey, substitution));
        }

        public void RegisterFunc<TOut>(Func<TOut> func, object serviceKey = null, Substitution substitution = Substitution.Forbidden)
        {
            AddFactory(new FuncFactory<TOut>(func, serviceKey, substitution));
        }

        public void RegisterFunc<TIn, TOut>(Func<TIn, TOut> func, object serviceKey = null, Substitution substitution = Substitution.Forbidden)
        {
            AddFactory(new FuncFactory<TIn, TOut>(func, serviceKey, substitution));
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

        public void Register<TAbstract, TImplmentation>(Subsistence subsistence = Subsistence.Transient,
            object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden)
            where TImplmentation : TAbstract
        {
            Register(typeof(TAbstract), typeof(TImplmentation), subsistence, serviceKey, constructor, substitution);
        }

        public void Register(Type abstractType, Type implementationType, Subsistence subsistence = Subsistence.Transient,
            object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden)
        {
            if (subsistence == Subsistence.Singleton)
                AddFactory(new SingletonFactory(abstractType, serviceKey,
                    constructor ?? GetDefaultConstructor(implementationType), substitution));
            else
                AddFactory(new TransientFactory(abstractType, serviceKey,
                    constructor ?? GetDefaultConstructor(implementationType), substitution));
        }

        public void Link<TRegistered, TLinked>(object registeredKey = null, object serviceKey = null,
            Substitution substitution = Substitution.Forbidden)
            where TLinked : TRegistered
        {
            Link(typeof(TRegistered), typeof(TLinked), registeredKey, serviceKey, substitution);
        }

        public void Link(Type registeredType, Type linkedType, object registeredKey = null, object serviceKey = null,
            Substitution substitution = Substitution.Forbidden)
        {
            AddFactory(new LinkedFactory(linkedType, this[registeredType, registeredKey], serviceKey, substitution));
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
            {
                if (_defaultFactories[type].Substitution == Substitution.Forbidden)
                    throw new AlreadyRegisterException(type);
                
                _defaultFactories.Remove(type);
            }

            _defaultFactories.Add(type, factory);
        }

        private void AddToKeyedFactory(IDependencyFactory factory)
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
    }
}