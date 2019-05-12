using Niddle.Resolvables.Base;
using Niddle.Utils;

namespace Niddle.Resolvables
{
    public class Resolvable : SingleResolvableBase, IResolvable<object>
    {
        public override object Resolve(IDependencyResolver resolver)
        {
            return resolver.Resolve(Type, Key, InstanceOrigins, resolver, AdditionalArguments);
        }

        public override bool TryResolve(IDependencyResolver resolver, out object value)
        {
            return resolver.TryResolve(out value, Type, Key, InstanceOrigins, resolver, AdditionalArguments);
        }

        IOptional<object> IResolvable<object>.TryResolve(IDependencyResolver resolver)
        {
            return TryResolve(resolver, out object value)
                ? new Optional<object>(value)
                : Optional<object>.NoValue;
        }
    }

    public class Resolvable<TValue> : SingleResolvableBase<TValue>
    {
        public override TValue Resolve(IDependencyResolver resolver)
        {
            return resolver.Resolve<TValue>(Key, InstanceOrigins, resolver, AdditionalArguments);
        }

        public override IOptional<TValue> TryResolve(IDependencyResolver resolver)
        {
            return resolver.TryResolve(out TValue value, Key, InstanceOrigins, resolver, AdditionalArguments)
                ? value
                : Optional<TValue>.NoValue;
        }
    }
}