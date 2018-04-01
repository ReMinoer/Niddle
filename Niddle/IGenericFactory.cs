using System;

namespace Niddle
{
    public interface IGenericFactory : IInjectionService
    {
        IDependencyFactory GetFactory(Type[] genericTypeArguments);
    }
}