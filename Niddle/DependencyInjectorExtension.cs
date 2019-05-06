using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Niddle.Factories;

namespace Niddle
{
    static public class DependencyInjectorExtension
    {
        static public IDependencyInjector With<TAbstract, TImplementation>(this IDependencyInjector injector, ConstructorInfo constructor = null)
            where TImplementation : TAbstract
            => With(injector, typeof(TAbstract), typeof(TImplementation), constructor);

        static public IDependencyInjector With(this IDependencyInjector injector, Type abstractType, Type implementationType, ConstructorInfo constructor = null)
            => With(injector, abstractType, constructor ?? GetDefaultConstructor(implementationType));

        static public IDependencyInjector With<T>(this IDependencyInjector injector, ConstructorInfo constructor = null)
            => With(injector, typeof(T), constructor);

        static public IDependencyInjector With(this IDependencyInjector injector, Type type, ConstructorInfo constructor = null)
        {
            return new DepedencyInjectorFactoryExtension(injector, new NewInstanceFactory(type, null, (constructor ?? GetDefaultConstructor(type)).AsResolvableRejecter(), Substitution.Forbidden));
        }
        
        static public IDependencyInjector WithSingleton<TAbstract, TImplementation>(this IDependencyInjector injector, ConstructorInfo constructor = null)
            where TImplementation : TAbstract
            => WithSingleton(injector, typeof(TAbstract), typeof(TImplementation), constructor);

        static public IDependencyInjector WithSingleton(this IDependencyInjector injector, Type abstractType, Type implementationType, ConstructorInfo constructor = null)
            => WithSingleton(injector, abstractType, constructor ?? GetDefaultConstructor(implementationType));

        static public IDependencyInjector WithSingleton<T>(this IDependencyInjector injector, ConstructorInfo constructor = null)
            => WithSingleton(injector, typeof(T), constructor);

        static public IDependencyInjector WithSingleton(this IDependencyInjector injector, Type type, ConstructorInfo constructor = null)
        {
            return new DepedencyInjectorFactoryExtension(injector, new SingletonFactory(type, null, (constructor ?? GetDefaultConstructor(type)).AsResolvableRejecter(), Substitution.Forbidden));
        }

        static public IDependencyInjector WithGeneric(this IDependencyInjector injector, Type genericTypeDescription, ConstructorInfo constructor = null)
            => WithGeneric(injector, genericTypeDescription, genericTypeDescription, constructor);

        static public IDependencyInjector WithGeneric(this IDependencyInjector injector, Type abstractTypeDescription, Type genericTypeDescription, ConstructorInfo constructor = null)
        {
            return new DepedencyInjectorGenericExtension(injector, new GenericFactory(abstractTypeDescription, genericTypeDescription, InstanceOrigin.Instantiation, null,
                constructor ?? GetDefaultConstructor(genericTypeDescription), Substitution.Forbidden));
        }

        static public IDependencyInjector WithGenericSingleton(this IDependencyInjector injector, Type genericTypeDescription, ConstructorInfo constructor = null)
            => WithGenericSingleton(injector, genericTypeDescription, genericTypeDescription, constructor);

        static public IDependencyInjector WithGenericSingleton(this IDependencyInjector injector, Type abstractTypeDescription, Type genericTypeDescription, ConstructorInfo constructor = null)
        {
            return new DepedencyInjectorGenericExtension(injector, new GenericFactory(abstractTypeDescription, genericTypeDescription, InstanceOrigin.Registration, null,
                constructor ?? GetDefaultConstructor(genericTypeDescription), Substitution.Forbidden));
        }

        static public IDependencyInjector WithInstance<TAbstract>(this IDependencyInjector injector, TAbstract instance)
        {
            return new DepedencyInjectorFactoryExtension(injector, new InstanceFactory(typeof(TAbstract), instance, null, Substitution.Forbidden));
        }

        static public IDependencyInjector WithInstance(this IDependencyInjector injector, Type type, object instance)
        {
            return new DepedencyInjectorFactoryExtension(injector, new InstanceFactory(type, instance, null, Substitution.Forbidden));
        }

        static public IDependencyInjector WithLazy<T>(this IDependencyInjector injector, Func<T> factory)
        {
            return new DepedencyInjectorFactoryExtension(injector, new LazyFactory<T>(factory, null, Substitution.Forbidden));
        }

        static public IDependencyInjector WithAction<TIn>(this IDependencyInjector injector, Action<TIn> action)
        {
            return new DepedencyInjectorFactoryExtension(injector, new ActionFactory<TIn>(action, null, Substitution.Forbidden));
        }

        static public IDependencyInjector WithFunc<TOut>(this IDependencyInjector injector, Func<TOut> func)
        {
            return new DepedencyInjectorFactoryExtension(injector, new FuncFactory<TOut>(func, null, Substitution.Forbidden));
        }

        static public IDependencyInjector WithFunc<TIn, TOut>(this IDependencyInjector injector, Func<TIn, TOut> func)
        {
            return new DepedencyInjectorFactoryExtension(injector, new FuncFactory<TIn, TOut>(func, null, Substitution.Forbidden));
        }

        static public IDependencyInjector WithLink<TLinked, TRegistered>(this IDependencyInjector injector, object registeredKey = null)
            where TLinked : TRegistered
        {
            return new DepedencyInjectorLinkExtension(injector, typeof(TLinked), typeof(TRegistered), registeredKey);
        }

        static public IDependencyInjector WithLink(this IDependencyInjector injector, Type linkedType, Type registeredType, object registeredKey = null)
        {
            return new DepedencyInjectorLinkExtension(injector, linkedType, registeredType, registeredKey);
        }

        static public IDependencyInjector WithGenericLink(this IDependencyInjector injector, Type linkedTypeDescription, Type registeredTypeDescription, object registeredKey = null)
        {
            return new DepedencyInjectorGenericLinkExtension(injector, linkedTypeDescription, registeredTypeDescription, registeredKey);
        }

        static private ConstructorInfo GetDefaultConstructor(Type type)
        {
            return type.GetTypeInfo().DeclaredConstructors.Where(x => x.IsPublic).Aggregate((min, next) => next.GetParameters().Length < min.GetParameters().Length ? next : min);
        }
    }

    public class DepedencyInjectorFactoryExtension : DepedencyInjectorExtensionBase
    {
        public IDependencyFactory Factory { get; }

        public DepedencyInjectorFactoryExtension(IDependencyInjector injector, IDependencyFactory factory)
            : base(injector)
        {
            Factory = factory;
        }

        protected override bool CheckType(Type type)
        {
            return type == Factory.Type;
        }

        protected override object ResolveExtension(Type type, IDependencyInjector dependencyInjector, IEnumerable<object> args = null)
        {
            return Factory.Get(dependencyInjector ?? this);
        }
    }

    public class DepedencyInjectorGenericExtension : DepedencyInjectorExtensionBase
    {
        public IGenericFactory Factory { get; }

        public DepedencyInjectorGenericExtension(IDependencyInjector injector, IGenericFactory factory)
            : base(injector)
        {
            Factory = factory;
        }

        protected override bool CheckType(Type type)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == Factory.Type;
        }

        protected override object ResolveExtension(Type type, IDependencyInjector dependencyInjector, IEnumerable<object> args = null)
        {
            return Factory.GetFactory(type.GenericTypeArguments).Get(dependencyInjector ?? this);
        }
    }

    public class DepedencyInjectorLinkExtension : DepedencyInjectorExtensionBase
    {
        public Type LinkedType { get; }
        public Type RegisteredType { get; }
        public object RegisteredKey { get; }

        public DepedencyInjectorLinkExtension(IDependencyInjector injector, Type linkedType, Type registeredType, object registeredKey = null)
            : base(injector)
        {
            LinkedType = linkedType;
            RegisteredType = registeredType;
            RegisteredKey = registeredKey;
        }

        protected override bool CheckType(Type type)
        {
            return type == LinkedType;
        }

        protected override object ResolveExtension(Type type, IDependencyInjector dependencyInjector, IEnumerable<object> args = null)
        {
            return Injector.Resolve(RegisteredType, RegisteredKey, InstanceOrigins.All, dependencyInjector ?? this, args);
        }
    }

    public class DepedencyInjectorGenericLinkExtension : DepedencyInjectorExtensionBase
    {
        public Type LinkedTypeDescription { get; }
        public Type RegisteredTypeDescription { get; }
        public object RegisteredKey { get; }

        public DepedencyInjectorGenericLinkExtension(IDependencyInjector injector, Type linkedTypeDescription, Type registeredTypeDescription, object registeredKey = null)
            : base(injector)
        {
            LinkedTypeDescription = linkedTypeDescription;
            RegisteredTypeDescription = registeredTypeDescription;
            RegisteredKey = registeredKey;
        }

        protected override bool CheckType(Type type)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == LinkedTypeDescription;
        }

        protected override object ResolveExtension(Type type, IDependencyInjector dependencyInjector, IEnumerable<object> args = null)
        {
            return Injector.Resolve(RegisteredTypeDescription.MakeGenericType(type.GenericTypeArguments), RegisteredKey, InstanceOrigins.All, dependencyInjector ?? this, args);
        }
    }

    public abstract class DepedencyInjectorExtensionBase : IDependencyInjector
    {
        protected readonly IDependencyInjector Injector;

        protected DepedencyInjectorExtensionBase(IDependencyInjector injector)
        {
            Injector = injector;
        }

        protected abstract bool CheckType(Type type);
        protected abstract object ResolveExtension(Type type, IDependencyInjector dependencyInjector, IEnumerable<object> args = null);

        public T Resolve<T>(object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null, IEnumerable<object> args = null)
        {
            if (CheckType(typeof(T)))
                return (T)ResolveExtension(typeof(T), dependencyInjector ?? this, args);

            return Injector.Resolve<T>(key, origins, dependencyInjector ?? this, args);
        }

        public object Resolve(Type type, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null, IEnumerable<object> args = null)
        {
            if (CheckType(type))
                return ResolveExtension(type, dependencyInjector ?? this, args);

            return Injector.Resolve(type, key, origins, dependencyInjector ?? this, args);
        }

        public IEnumerable<T> ResolveMany<T>(object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null, IEnumerable<object> args = null)
        {
            if (CheckType(typeof(T)))
                yield return (T)ResolveExtension(typeof(T), dependencyInjector ?? this, args);

            foreach (T item in Injector.ResolveMany<T>(key, origins, dependencyInjector ?? this, args))
                yield return item;
        }

        public IEnumerable ResolveMany(Type type, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null, IEnumerable<object> args = null)
        {
            if (CheckType(type))
                yield return ResolveExtension(type, dependencyInjector ?? this, args);

            foreach (object item in Injector.ResolveMany(type, key, origins, dependencyInjector ?? this, args))
                yield return item;
        }

        public bool TryResolve<T>(out T obj, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null, IEnumerable<object> args = null)
        {
            if (CheckType(typeof(T)))
            {
                obj = (T)ResolveExtension(typeof(T), dependencyInjector ?? this, args);
                return true;
            }

            return Injector.TryResolve(out obj, key, origins, dependencyInjector ?? this, args);
        }

        public bool TryResolve(out object obj, Type type, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null, IEnumerable<object> args = null)
        {
            if (CheckType(type))
            {
                obj = ResolveExtension(type, dependencyInjector ?? this, args);
                return true;
            }

            return Injector.TryResolve(out obj, type, key, origins, dependencyInjector ?? this, args);
        }

        public bool TryResolveMany<T>(out IEnumerable<T> objs, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null, IEnumerable<object> args = null)
        {
            if (CheckType(typeof(T)))
            {
                objs = new[] { (T)ResolveExtension(typeof(T), dependencyInjector ?? this, args) };
                return true;
            }

            return Injector.TryResolveMany(out objs, key, origins, dependencyInjector ?? this, args);
        }

        public bool TryResolveMany(out IEnumerable objs, Type type, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null, IEnumerable<object> args = null)
        {
            if (CheckType(type))
            {
                objs = new[] { ResolveExtension(type, dependencyInjector ?? this, args) };
                return true;
            }

            return Injector.TryResolveMany(out objs, type, key, origins, dependencyInjector ?? this, args);
        }
    }
}