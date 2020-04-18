using System;
using System.Collections.Generic;

namespace Niddle.Dependencies
{
    public interface IGenericFactory : IFactory
    {
        IDependencyFactory GetFactory(IEnumerable<Type> genericTypeArguments);
    }
}