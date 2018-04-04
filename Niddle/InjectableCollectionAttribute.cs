using System;
using System.Collections.Generic;
using Niddle.Base;

namespace Niddle
{
    public class InjectableCollectionAttribute : InjectableManyByGenericMethodAttributeBase
    {
        protected override Type GenericTypeDefinition => typeof(ICollection<>);
        protected override string MethodName => nameof(ICollection<object>.Add);
    }
}