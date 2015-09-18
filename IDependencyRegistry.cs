using System;
using System.Reflection;

namespace Diese.Injection
{
    public interface IDependencyRegistry
    {
        IDependencyFactory this[Type type, object serviceKey = null] { get; }
        void RegisterInstance<TAbstract>(object instance, object serviceKey = null, Substitution substitution = Substitution.Forbidden);
        void RegisterInstance(Type abstractType, object instance, object serviceKey = null, Substitution substitution = Substitution.Forbidden);
        void RegisterLazy<T>(Func<T> factory, object serviceKey = null, Substitution substitution = Substitution.Forbidden);
        void RegisterAction<TIn>(Action<TIn> action, object serviceKey = null, Substitution substitution = Substitution.Forbidden);
        void RegisterFunc<TOut>(Func<TOut> func, object serviceKey = null, Substitution substitution = Substitution.Forbidden);
        void RegisterFunc<TIn, TOut>(Func<TIn, TOut> func, object serviceKey = null, Substitution substitution = Substitution.Forbidden);

        void Register<T>(Subsistence subsistence = Subsistence.Transient, object serviceKey = null,
            ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden);

        void Register(Type type, Subsistence subsistence = Subsistence.Transient, object serviceKey = null,
            ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden);

        void Register<TAbstract, TImplmentation>(Subsistence subsistence = Subsistence.Transient,
            object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden)
            where TImplmentation : TAbstract;

        void Register(Type abstractType, Type implementationType, Subsistence subsistence = Subsistence.Transient,
            object serviceKey = null, ConstructorInfo constructor = null, Substitution substitution = Substitution.Forbidden);

        void Link<TLinked, TRegistered>(object registeredKey = null, object serviceKey = null,
            Substitution substitution = Substitution.Forbidden)
            where TRegistered : TLinked;

        void Link(Type linkedType, Type registeredType, object registeredKey = null, object serviceKey = null,
            Substitution substitution = Substitution.Forbidden);
    }
}