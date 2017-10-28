using System;
using System.Reflection;

namespace Diese.Injection
{
    public interface IDependencyRegistry
    {
        IDependencyFactory this[Type type, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All] { get; }
        bool TryGetFactory(out IDependencyFactory factory, Type type, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All);

        void Register<TAbstract, TImplementation>(object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden)
            where TImplementation : TAbstract;
        void Register(Type abstractType, Type implementationType, object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden);
        void Register<T>(object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden);
        void Register(Type type, object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden);

        void RegisterSingleton<TAbstract, TImplementation>(object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden)
            where TImplementation : TAbstract;
        void RegisterSingleton(Type abstractType, Type implementationType, object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden);
        void RegisterSingleton<T>(object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden);
        void RegisterSingleton(Type type, object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden);

        void RegisterGeneric(Type genericTypeDescription, object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden);
        void RegisterGeneric(Type abstractTypeDescription, Type genericTypeDescription, object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden);

        void RegisterGenericSingleton(Type genericTypeDescription, object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden);
        void RegisterGenericSingleton(Type abstractTypeDescription, Type genericTypeDescription, object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden);

        void RegisterInstance<TAbstract>(TAbstract instance, object serviceKey = null, Substitution substitution = Substitution.Forbidden);
        void RegisterInstance(Type abstractType, object instance, object serviceKey = null, Substitution substitution = Substitution.Forbidden);
        void RegisterLazy<T>(Func<T> factory, object serviceKey = null, Substitution substitution = Substitution.Forbidden);
        void RegisterAction<TIn>(Action<TIn> action, object serviceKey = null, Substitution substitution = Substitution.Forbidden);
        void RegisterFunc<TOut>(Func<TOut> func, object serviceKey = null, Substitution substitution = Substitution.Forbidden);
        void RegisterFunc<TIn, TOut>(Func<TIn, TOut> func, object serviceKey = null, Substitution substitution = Substitution.Forbidden);

        void Link<TLinked, TRegistered>(object registeredKey = null, object serviceKey = null,
            Substitution substitution = Substitution.Forbidden)
            where TRegistered : TLinked;

        void Link(Type linkedType, Type registeredType, object registeredKey = null, object serviceKey = null,
            Substitution substitution = Substitution.Forbidden);

        void LinkGeneric(Type linkedTypeDescription, Type registeredTypeDescription, object registeredKey = null, object serviceKey = null,
            Substitution substitution = Substitution.Forbidden);
    }
}