using Niddle.Resolvables.Base;

namespace Niddle.Resolvables
{
    public class Resolvable : SingleResolvableBase, IResolvable<object>
    {
        public override object Resolve(IDependencyInjector injector)
        {
            return injector.Resolve(Type, Key, InstanceOrigins, injector, AdditionalArguments);
        }

        public override bool TryResolve(IDependencyInjector injector, out object value)
        {
            return injector.TryResolve(out value, Type, Key, InstanceOrigins, injector, AdditionalArguments);
        }
    }

    public class Resolvable<TValue> : SingleResolvableBase<TValue>
    {
        public override TValue Resolve(IDependencyInjector injector)
        {
            return injector.Resolve<TValue>(Key, InstanceOrigins, injector, AdditionalArguments);
        }

        public override bool TryResolve(IDependencyInjector injector, out TValue value)
        {
            return injector.TryResolve(out value, Key, InstanceOrigins, injector, AdditionalArguments);
        }
    }
}