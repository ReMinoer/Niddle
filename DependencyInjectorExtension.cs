using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Diese.Injection.Factories;

namespace Diese.Injection
{
    static public class DependencyInjectorExtension
    {
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

        static public IDependencyInjector WithGeneric(this IDependencyInjector injector, Type genericTypeDescription, Subsistence subsistence = Subsistence.Transient, ConstructorInfo constructor = null)
        {
            return WithGeneric(injector, genericTypeDescription, genericTypeDescription, subsistence, constructor);
        }

        static public IDependencyInjector WithGeneric(this IDependencyInjector injector, Type abstractTypeDescription, Type genericTypeDescription, Subsistence subsistence = Subsistence.Transient, ConstructorInfo constructor = null)
        {
            return new DepedencyInjectorGenericExtension(injector, new GenericFactory(abstractTypeDescription, subsistence, null,
                constructor ?? GetDefaultConstructor(genericTypeDescription), Substitution.Forbidden));
        }

        static public IDependencyInjector With<TAbstract, TImplementation>(this IDependencyInjector injector, Subsistence subsistence = Subsistence.Transient, ConstructorInfo constructor = null)
            where TImplementation : TAbstract
        {
            return With(injector, typeof(TAbstract), typeof(TImplementation), subsistence, constructor);
        }

        static public IDependencyInjector With(this IDependencyInjector injector, Type abstractType, Type implementationType, Subsistence subsistence = Subsistence.Transient, ConstructorInfo constructor = null)
        {
            return With(injector, abstractType, subsistence, constructor ?? GetDefaultConstructor(implementationType));
        }

        static public IDependencyInjector With<T>(this IDependencyInjector injector, Subsistence subsistence = Subsistence.Transient, ConstructorInfo constructor = null)
        {
            return With(injector, typeof(T), subsistence, constructor);
        }

        static public IDependencyInjector With(this IDependencyInjector injector, Type type, Subsistence subsistence = Subsistence.Transient, ConstructorInfo constructor = null)
        {
            if (subsistence == Subsistence.Singleton)
                return new DepedencyInjectorFactoryExtension(injector, new SingletonFactory(type, null, constructor ?? GetDefaultConstructor(type), Substitution.Forbidden));
            else
                return new DepedencyInjectorFactoryExtension(injector, new TransientFactory(type, null, constructor ?? GetDefaultConstructor(type), Substitution.Forbidden));
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
            return type.GetConstructors().Aggregate((min, next) => next.GetParameters().Length < min.GetParameters().Length ? next : min);
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

        protected override object ResolveExtension(Type type, IDependencyInjector dependencyInjector)
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
            return type.IsGenericType && type.GetGenericTypeDefinition() == Factory.Type;
        }

        protected override object ResolveExtension(Type type, IDependencyInjector dependencyInjector)
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

        protected override object ResolveExtension(Type type, IDependencyInjector dependencyInjector)
        {
            return Injector.Resolve(RegisteredType, null, RegisteredKey, dependencyInjector ?? this);
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
            return type.IsGenericType && type.GetGenericTypeDefinition() == LinkedTypeDescription;
        }

        protected override object ResolveExtension(Type type, IDependencyInjector dependencyInjector)
        {
            return Injector.Resolve(RegisteredTypeDescription.MakeGenericType(type.GenericTypeArguments), null, RegisteredKey, dependencyInjector ?? this);
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
        protected abstract object ResolveExtension(Type type, IDependencyInjector dependencyInjector);

        public T Resolve<T>(InjectableAttributeBase injectableAttribute = null, object serviceKey = null, IDependencyInjector dependencyInjector = null)
        {
            if (CheckType(typeof(T)))
                return (T)ResolveExtension(typeof(T), dependencyInjector ?? this);

            return Injector.Resolve<T>(injectableAttribute, serviceKey, dependencyInjector ?? this);
        }

        public object Resolve(Type type, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, IDependencyInjector dependencyInjector = null)
        {
            if (CheckType(type))
                return ResolveExtension(type, dependencyInjector ?? this);

            return Injector.Resolve(type, injectableAttribute, serviceKey, dependencyInjector ?? this);
        }

        public IEnumerable<T> ResolveMany<T>(InjectableAttributeBase injectableAttribute = null, object serviceKey = null, IDependencyInjector dependencyInjector = null)
        {
            if (CheckType(typeof(T)))
                yield return (T)ResolveExtension(typeof(T), dependencyInjector ?? this);

            foreach (T item in Injector.ResolveMany<T>(injectableAttribute, serviceKey, dependencyInjector ?? this))
                yield return item;
        }

        public IEnumerable ResolveMany(Type type, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, IDependencyInjector dependencyInjector = null)
        {
            if (CheckType(type))
                yield return ResolveExtension(type, dependencyInjector ?? this);

            foreach (object item in Injector.ResolveMany(type, injectableAttribute, serviceKey, dependencyInjector ?? this))
                yield return item;
        }

        public bool TryResolve<T>(out T obj, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, IDependencyInjector dependencyInjector = null)
        {
            if (CheckType(typeof(T)))
            {
                obj = (T)ResolveExtension(typeof(T), dependencyInjector ?? this);
                return true;
            }

            return Injector.TryResolve(out obj, injectableAttribute, serviceKey, dependencyInjector ?? this);
        }

        public bool TryResolve(out object obj, Type type, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, IDependencyInjector dependencyInjector = null)
        {
            if (CheckType(type))
            {
                obj = ResolveExtension(type, dependencyInjector ?? this);
                return true;
            }

            return Injector.TryResolve(out obj, type, injectableAttribute, serviceKey, dependencyInjector ?? this);
        }

        public bool TryResolveMany<T>(out IEnumerable<T> objs, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, IDependencyInjector dependencyInjector = null)
        {
            if (CheckType(typeof(T)))
            {
                objs = new[] { (T)ResolveExtension(typeof(T), dependencyInjector ?? this) };
                return true;
            }

            return Injector.TryResolveMany(out objs, injectableAttribute, serviceKey, dependencyInjector ?? this);
        }

        public bool TryResolveMany(out IEnumerable objs, Type type, InjectableAttributeBase injectableAttribute = null, object serviceKey = null, IDependencyInjector dependencyInjector = null)
        {
            if (CheckType(type))
            {
                objs = new[] { ResolveExtension(type, dependencyInjector ?? this) };
                return true;
            }

            return Injector.TryResolveMany(out objs, type, injectableAttribute, serviceKey, dependencyInjector ?? this);
        }
    }
}