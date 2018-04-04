using System;
using System.Collections.Generic;

namespace Niddle
{
    public interface IInjectableManyAttribute : IInjectableAttribute
    {
        IEnumerable<Type> GetInjectableTypes(Type memberType);
    }
}