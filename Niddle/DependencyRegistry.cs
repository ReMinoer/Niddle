using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Niddle.Exceptions;

namespace Niddle
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

        public IDependencyFactory this[Type genericTypeDefinition, IEnumerable<Type> genericTypeArguments, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All]
        {
            get
            {
                if (TryGetFactory(out IDependencyFactory factory, genericTypeDefinition, genericTypeArguments, serviceKey, instanceOrigins))
                    return factory;

                throw new NotRegisterException(genericTypeDefinition, serviceKey);
            }
        }

        public bool TryGetFactory(out IDependencyFactory factory, Type type, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All)
        {
            factory = _dependencyFactories[type, serviceKey, instanceOrigins];
            if (factory != null)
                return true;

            return type.GetTypeInfo().IsGenericType && TryGetFactory(out factory, type.GetGenericTypeDefinition(), type.GenericTypeArguments, serviceKey, instanceOrigins);
        }

        public bool TryGetFactory(out IDependencyFactory factory, Type genericTypeDefinition, IEnumerable<Type> genericTypeArguments, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All)
        {
            IGenericFactory genericFactory = _genericFactories[genericTypeDefinition, serviceKey, instanceOrigins];
            factory = genericFactory?.GetFactory(genericTypeArguments);

            return factory != null;
        }

        public void Add(IDependencyFactory factory)
        {
            if (factory.ServiceKey != null)
                _dependencyFactories.AddToKeyedFactories(factory);
            else
                _dependencyFactories.AddToDefaultFactories(factory);
        }

        public void Add(IGenericFactory genericFactory)
        {
            if (genericFactory.ServiceKey != null)
                _genericFactories.AddToKeyedFactories(genericFactory);
            else
                _genericFactories.AddToDefaultFactories(genericFactory);
        }

        public IEnumerator<IInjectionService> GetEnumerator()
        {
            return Enumerable.Concat<IInjectionService>(_dependencyFactories, _genericFactories).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class OriginFactories<TFactory> : IEnumerable<TFactory>
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
            }

            public TFactory this[InstanceOrigin? instanceOrigin]
            {
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
                        case null:
                            _instantiationFactory = value;
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
                    if ((instanceOrigins & InstanceOrigins.Registration) != 0)
                    {
                        if (_registrationFactory != null)
                            return _registrationFactory;
                    }

                    if ((instanceOrigins & InstanceOrigins.Instantiation) != 0)
                    {
                        if (_instantiationFactory != null)
                            return _instantiationFactory;
                    }

                    return null;
                }
            }

            public IEnumerator<TFactory> GetEnumerator()
            {
                return Enumerate().GetEnumerator();

                IEnumerable<TFactory> Enumerate()
                {
                    if (_registrationFactory != null)
                        yield return _registrationFactory;
                    if (_instantiationFactory != null)
                        yield return _instantiationFactory;
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        internal sealed class KeyableServiceRegistry<TFactory> : IEnumerable<TFactory>
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

                if (_defaultFactories.TryGetValue(type, out OriginFactories<TFactory> originFactories))
                {
                    if (factory.InstanceOrigin != null)
                    {
                        TFactory originFactory = originFactories[factory.InstanceOrigin.Value];
                        if (originFactory.InstanceOrigin != null && originFactory.Substitution == Substitution.Forbidden)
                            throw new AlreadyRegisterException(type);
                    }
                }
                else
                {
                    originFactories = new OriginFactories<TFactory>();
                    _defaultFactories[type] = originFactories;
                }

                originFactories[factory.InstanceOrigin] = factory;
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
                        if (factory.InstanceOrigin != null)
                        {
                            TFactory originFactory = originFactories[factory.InstanceOrigin.Value];
                            if (originFactory.InstanceOrigin != null && originFactory.Substitution == Substitution.Forbidden)
                                throw new AlreadyRegisterException(serviceKey);
                        }
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

            public IEnumerator<TFactory> GetEnumerator()
            {
                return Enumerable.Concat(_defaultFactories.Values.SelectMany(x => x), _keyedFactories.Values.SelectMany(x => x.Values.SelectMany(y => y))).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}