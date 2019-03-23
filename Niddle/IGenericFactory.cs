using System;
using System.Collections.Generic;

namespace Niddle
{
    public interface IGenericFactory : IInjectionService
    {
        IDependencyFactory GetFactory(IEnumerable<Type> genericTypeArguments);
    }
}