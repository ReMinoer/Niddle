namespace Niddle.Resolvables.Base
{
    public abstract class ResolvableBase<TValue> : IResolvable<TValue>
    {
        public abstract TValue Resolve(IDependencyResolver resolver);
        public abstract IOptional<TValue> TryResolve(IDependencyResolver resolver);

        object IResolvable.Resolve(IDependencyResolver resolver) => Resolve(resolver);
        public bool TryResolve(IDependencyResolver resolver, out object value)
        {
            IOptional<TValue> resolvedValue = TryResolve(resolver);
            if (resolvedValue.HasValue)
            {
                value = resolvedValue.Value;
                return true;
            }

            value = null;
            return false;
        }
    }
}