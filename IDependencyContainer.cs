using System;
using System.Reflection;

namespace Diese.Injection
{
    public interface IDependencyContainer
    {
        void Register<T>(Subsistence singleton = Subsistence.Transient, object key = null, ConstructorInfo constructor = null);
        void Register<TAbstract, TImplmentation>(Subsistence singleton = Subsistence.Transient, object key = null, ConstructorInfo constructor = null)
            where TImplmentation : TAbstract;
        void Register<TAbstract>(Type implementationType, Subsistence singleton = Subsistence.Transient, object key = null, ConstructorInfo constructor = null);
        void Register(Type abstractType, Type implementationType, Subsistence singleton = Subsistence.Transient, object key = null, ConstructorInfo constructor = null);
        
        void RegisterInstance<TAbstract>(object instance);
    }
}