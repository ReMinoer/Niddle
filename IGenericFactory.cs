using System;

namespace Diese.Injection
{
    public interface IGenericFactory : IServiceFactory
    {
        IDependencyFactory GetFactory(Type[] genericTypeArguments);
    }
}