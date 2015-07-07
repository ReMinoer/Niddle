using System;
using System.Reflection;

namespace Diese.Injection
{
    public interface IDependencyRegistry
    {
        IDependencyFactory this[Type type, object serviceKey = null] { get; }

        void RegisterInstance<TAbstract>(object instance, object serviceKey = null);
        void RegisterInstance(Type abstractType, object instance, object serviceKey = null);

        void Register<T>(Subsistence subsistence = Subsistence.Transient, object serviceKey = null, ConstructorInfo constructor = null);
        void Register(Type type, Subsistence subsistence = Subsistence.Transient, object serviceKey = null, ConstructorInfo constructor = null);
        
        void Register<TAbstract, TImplmentation>(Subsistence subsistence = Subsistence.Transient, object serviceKey = null, ConstructorInfo constructor = null)
            where TImplmentation : TAbstract;
        void Register<TAbstract>(Type implementationType, Subsistence subsistence = Subsistence.Transient, object serviceKey = null, ConstructorInfo constructor = null);
        void Register(Type abstractType, Type implementationType, Subsistence subsistence = Subsistence.Transient, object serviceKey = null, ConstructorInfo constructor = null);
    }
}