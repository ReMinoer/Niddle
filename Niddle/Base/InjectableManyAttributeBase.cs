using System;
using System.Collections.Generic;

namespace Niddle.Base
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public abstract class InjectableManyAttributeBase : InjectableAttributeBase, IInjectableManyAttribute
    {
        public abstract IEnumerable<Type> GetInjectableTypes(Type memberType);
    }
}