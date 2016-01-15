using System;

namespace Diese.Injection
{
    public interface IGenericFactory : IInjectionService
    {
        IDependencyFactory GetFactory(Type[] genericTypeArguments);
    }
}