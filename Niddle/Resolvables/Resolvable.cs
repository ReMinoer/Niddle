using Niddle.Resolvables.Base;

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
    }

    public class Resolvable<TValue> : SingleResolvableBase<TValue>
    {
        public override TValue Resolve(IDependencyResolver resolver)
        {
            return resolver.Resolve<TValue>(Key, InstanceOrigins, resolver, AdditionalArguments);
        }

        public override bool TryResolve(IDependencyResolver resolver, out TValue value)
        {
            return resolver.TryResolve(out value, Key, InstanceOrigins, resolver, AdditionalArguments);
        }
    }
}