using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Niddle.Dependencies.Factories;

namespace Niddle
{
    static public class DependencyResolverExtraExtension
    {
        static public IDependencyResolver With<TAbstract, TImplementation>(this IDependencyResolver resolver, ConstructorInfo constructor = null)
            where TImplementation : TAbstract
            => With(resolver, typeof(TAbstract), typeof(TImplementation), constructor);

        static public IDependencyResolver With(this IDependencyResolver resolver, Type abstractType, Type implementationType, ConstructorInfo constructor = null)
            => With(resolver, abstractType, constructor ?? GetDefaultConstructor(implementationType));

        static public IDependencyResolver With<T>(this IDependencyResolver resolver, ConstructorInfo constructor = null)
            => With(resolver, typeof(T), constructor);

        static public IDependencyResolver With(this IDependencyResolver resolver, Type type, ConstructorInfo constructor = null)
        {
            return new DepedencyResolverFactoryExtra(resolver, new NewInstanceFactory(type, null, (constructor ?? GetDefaultConstructor(type)).AsResolvableRejecter(), Substitution.Forbidden));
        }
        
        static public IDependencyResolver WithSingleton<TAbstract, TImplementation>(this IDependencyResolver resolver, ConstructorInfo constructor = null)
            where TImplementation : TAbstract
            => WithSingleton(resolver, typeof(TAbstract), typeof(TImplementation), constructor);

        static public IDependencyResolver WithSingleton(this IDependencyResolver resolver, Type abstractType, Type implementationType, ConstructorInfo constructor = null)
            => WithSingleton(resolver, abstractType, constructor ?? GetDefaultConstructor(implementationType));

        static public IDependencyResolver WithSingleton<T>(this IDependencyResolver resolver, ConstructorInfo constructor = null)
            => WithSingleton(resolver, typeof(T), constructor);

        static public IDependencyResolver WithSingleton(this IDependencyResolver resolver, Type type, ConstructorInfo constructor = null)
        {
            return new DepedencyResolverFactoryExtra(resolver, new SingletonFactory(type, null, (constructor ?? GetDefaultConstructor(type)).AsResolvableRejecter(), Substitution.Forbidden));
        }

        static public IDependencyResolver WithGeneric(this IDependencyResolver resolver, Type genericTypeDescription, ConstructorInfo constructor = null)
            => WithGeneric(resolver, genericTypeDescription, genericTypeDescription, constructor);

        static public IDependencyResolver WithGeneric(this IDependencyResolver resolver, Type abstractTypeDescription, Type genericTypeDescription, ConstructorInfo constructor = null)
        {
            return new DepedencyResolverGenericExtra(resolver, new GenericFactory(abstractTypeDescription, genericTypeDescription, InstanceOrigin.Instantiation, null,
                constructor ?? GetDefaultConstructor(genericTypeDescription), Substitution.Forbidden));
        }

        static public IDependencyResolver WithGenericSingleton(this IDependencyResolver resolver, Type genericTypeDescription, ConstructorInfo constructor = null)
            => WithGenericSingleton(resolver, genericTypeDescription, genericTypeDescription, constructor);

        static public IDependencyResolver WithGenericSingleton(this IDependencyResolver resolver, Type abstractTypeDescription, Type genericTypeDescription, ConstructorInfo constructor = null)
        {
            return new DepedencyResolverGenericExtra(resolver, new GenericFactory(abstractTypeDescription, genericTypeDescription, InstanceOrigin.Registration, null,
                constructor ?? GetDefaultConstructor(genericTypeDescription), Substitution.Forbidden));
        }

        static public IDependencyResolver WithInstance<TAbstract>(this IDependencyResolver resolver, TAbstract instance)
        {
            return new DepedencyResolverFactoryExtra(resolver, new InstanceFactory(typeof(TAbstract), instance, null, Substitution.Forbidden));
        }

        static public IDependencyResolver WithInstance(this IDependencyResolver resolver, Type type, object instance)
        {
            return new DepedencyResolverFactoryExtra(resolver, new InstanceFactory(type, instance, null, Substitution.Forbidden));
        }

        static public IDependencyResolver WithLazy<T>(this IDependencyResolver resolver, Func<T> factory)
        {
            return new DepedencyResolverFactoryExtra(resolver, new LazyFactory<T>(factory, null, Substitution.Forbidden));
        }

        static public IDependencyResolver WithAction<TIn>(this IDependencyResolver resolver, Action<TIn> action)
        {
            return new DepedencyResolverFactoryExtra(resolver, new ActionFactory<TIn>(action, null, Substitution.Forbidden));
        }

        static public IDependencyResolver WithFunc<TOut>(this IDependencyResolver resolver, Func<TOut> func)
        {
            return new DepedencyResolverFactoryExtra(resolver, new FuncFactory<TOut>(func, null, Substitution.Forbidden));
        }

        static public IDependencyResolver WithFunc<TIn, TOut>(this IDependencyResolver resolver, Func<TIn, TOut> func)
        {
            return new DepedencyResolverFactoryExtra(resolver, new FuncFactory<TIn, TOut>(func, null, Substitution.Forbidden));
        }

        static public IDependencyResolver WithLink<TLinked, TRegistered>(this IDependencyResolver resolver, object registeredKey = null)
            where TLinked : TRegistered
        {
            return new DepedencyResolverLinkExtra(resolver, typeof(TLinked), typeof(TRegistered), registeredKey);
        }

        static public IDependencyResolver WithLink(this IDependencyResolver resolver, Type linkedType, Type registeredType, object registeredKey = null)
        {
            return new DepedencyResolverLinkExtra(resolver, linkedType, registeredType, registeredKey);
        }

        static public IDependencyResolver WithGenericLink(this IDependencyResolver resolver, Type linkedTypeDescription, Type registeredTypeDescription, object registeredKey = null)
        {
            return new DepedencyResolverGenericLinkExtra(resolver, linkedTypeDescription, registeredTypeDescription, registeredKey);
        }

        static private ConstructorInfo GetDefaultConstructor(Type type)
        {
            return type.GetTypeInfo().DeclaredConstructors.Where(x => x.IsPublic).Aggregate((min, next) => next.GetParameters().Length < min.GetParameters().Length ? next : min);
        }

        private class DepedencyResolverFactoryExtra : DepedencyResolverExtraBase
        {
            public IDependencyFactory Factory { get; }

            public DepedencyResolverFactoryExtra(IDependencyResolver resolver, IDependencyFactory factory)
                : base(resolver)
            {
                Factory = factory;
            }

            protected override bool CheckType(Type type)
            {
                return type == Factory.Type;
            }

            protected override object ResolveExtension(Type type, IDependencyResolver resolver, IEnumerable<object> args = null)
            {
                return Factory.Get(resolver ?? this);
            }
        }

        private class DepedencyResolverGenericExtra : DepedencyResolverExtraBase
        {
            public IGenericFactory Factory { get; }

            public DepedencyResolverGenericExtra(IDependencyResolver resolver, IGenericFactory factory)
                : base(resolver)
            {
                Factory = factory;
            }

            protected override bool CheckType(Type type)
            {
                return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == Factory.Type;
            }

            protected override object ResolveExtension(Type type, IDependencyResolver resolver, IEnumerable<object> args = null)
            {
                return Factory.GetFactory(type.GenericTypeArguments).Get(resolver ?? this);
            }
        }

        private class DepedencyResolverLinkExtra : DepedencyResolverExtraBase
        {
            public Type LinkedType { get; }
            public Type RegisteredType { get; }
            public object RegisteredKey { get; }

            public DepedencyResolverLinkExtra(IDependencyResolver resolver, Type linkedType, Type registeredType, object registeredKey = null)
                : base(resolver)
            {
                LinkedType = linkedType;
                RegisteredType = registeredType;
                RegisteredKey = registeredKey;
            }

            protected override bool CheckType(Type type)
            {
                return type == LinkedType;
            }

            protected override object ResolveExtension(Type type, IDependencyResolver resolver, IEnumerable<object> args = null)
            {
                return Resolver.Resolve(RegisteredType, RegisteredKey, InstanceOrigins.All, resolver ?? this, args);
            }
        }

        private class DepedencyResolverGenericLinkExtra : DepedencyResolverExtraBase
        {
            public Type LinkedTypeDescription { get; }
            public Type RegisteredTypeDescription { get; }
            public object RegisteredKey { get; }

            public DepedencyResolverGenericLinkExtra(IDependencyResolver resolver, Type linkedTypeDescription, Type registeredTypeDescription, object registeredKey = null)
                : base(resolver)
            {
                LinkedTypeDescription = linkedTypeDescription;
                RegisteredTypeDescription = registeredTypeDescription;
                RegisteredKey = registeredKey;
            }

            protected override bool CheckType(Type type)
            {
                return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == LinkedTypeDescription;
            }

            protected override object ResolveExtension(Type type, IDependencyResolver resolver, IEnumerable<object> args = null)
            {
                return Resolver.Resolve(RegisteredTypeDescription.MakeGenericType(type.GenericTypeArguments), RegisteredKey, InstanceOrigins.All, resolver ?? this, args);
            }
        }

        private abstract class DepedencyResolverExtraBase : IDependencyResolver
        {
            protected readonly IDependencyResolver Resolver;

            protected DepedencyResolverExtraBase(IDependencyResolver resolver)
            {
                Resolver = resolver;
            }

            protected abstract bool CheckType(Type type);
            protected abstract object ResolveExtension(Type type, IDependencyResolver resolver, IEnumerable<object> args = null);

            public T Resolve<T>(object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null, IEnumerable<object> args = null)
            {
                if (CheckType(typeof(T)))
                    return (T)ResolveExtension(typeof(T), resolver ?? this, args);

                return Resolver.Resolve<T>(key, origins, resolver ?? this, args);
            }

            public object Resolve(Type type, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null, IEnumerable<object> args = null)
            {
                if (CheckType(type))
                    return ResolveExtension(type, resolver ?? this, args);

                return Resolver.Resolve(type, key, origins, resolver ?? this, args);
            }

            public IEnumerable<T> ResolveMany<T>(object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null, IEnumerable<object> args = null)
            {
                if (CheckType(typeof(T)))
                    yield return (T)ResolveExtension(typeof(T), resolver ?? this, args);

                foreach (T item in Resolver.ResolveMany<T>(key, origins, resolver ?? this, args))
                    yield return item;
            }

            public IEnumerable ResolveMany(Type type, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null, IEnumerable<object> args = null)
            {
                if (CheckType(type))
                    yield return ResolveExtension(type, resolver ?? this, args);

                foreach (object item in Resolver.ResolveMany(type, key, origins, resolver ?? this, args))
                    yield return item;
            }

            public bool TryResolve<T>(out T obj, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null, IEnumerable<object> args = null)
            {
                if (CheckType(typeof(T)))
                {
                    obj = (T)ResolveExtension(typeof(T), resolver ?? this, args);
                    return true;
                }

                return Resolver.TryResolve(out obj, key, origins, resolver ?? this, args);
            }

            public bool TryResolve(out object obj, Type type, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null, IEnumerable<object> args = null)
            {
                if (CheckType(type))
                {
                    obj = ResolveExtension(type, resolver ?? this, args);
                    return true;
                }

                return Resolver.TryResolve(out obj, type, key, origins, resolver ?? this, args);
            }

            public bool TryResolveMany<T>(out IEnumerable<T> objs, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null, IEnumerable<object> args = null)
            {
                if (CheckType(typeof(T)))
                {
                    objs = new[] { (T)ResolveExtension(typeof(T), resolver ?? this, args) };
                    return true;
                }

                return Resolver.TryResolveMany(out objs, key, origins, resolver ?? this, args);
            }

            public bool TryResolveMany(out IEnumerable objs, Type type, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null, IEnumerable<object> args = null)
            {
                if (CheckType(type))
                {
                    objs = new[] { ResolveExtension(type, resolver ?? this, args) };
                    return true;
                }

                return Resolver.TryResolveMany(out objs, type, key, origins, resolver ?? this, args);
            }
        }
    }
}