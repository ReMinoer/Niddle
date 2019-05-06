namespace Niddle.Resolvables.Base
{
    public abstract class ResolvableBase<TValue> : IResolvable<TValue>
    {
        public abstract TValue Resolve(IDependencyInjector injector);
        public abstract bool TryResolve(IDependencyInjector injector, out TValue value);

        object IResolvable.Resolve(IDependencyInjector injector) => Resolve(injector);

        public bool TryResolve(IDependencyInjector injector, out object value)
        {
            if (TryResolve(injector, out TValue obj))
            {
                value = obj;
                return true;
            }

            value = null;
            return false;
        }
    }
}