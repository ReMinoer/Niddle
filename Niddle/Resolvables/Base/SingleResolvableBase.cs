using System;
using System.Collections.Generic;

namespace Niddle.Resolvables.Base
{
    public abstract class SingleResolvableBase : IResolvable
    {
        public Type Type { get; set; }
        public object Key { get; set; }
        public InstanceOrigins InstanceOrigins { get; set; } = InstanceOrigins.All;
        public IEnumerable<object> AdditionalArguments { get; set; }

        public abstract object Resolve(IDependencyInjector injector);
        public abstract bool TryResolve(IDependencyInjector injector, out object value);
    }

    public abstract class SingleResolvableBase<TValue> : ResolvableBase<TValue>
    {
        public object Key { get; set; }
        public InstanceOrigins InstanceOrigins { get; set; } = InstanceOrigins.All;
        public IEnumerable<object> AdditionalArguments { get; set; }
    }
}