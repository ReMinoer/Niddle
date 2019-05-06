namespace Niddle.Resolvables.Base
{
    public abstract class ResolvableBase<TValue> : IResolvable<TValue>
    {
        public abstract TValue Resolve(IDependencyResolver resolver);
        public abstract bool TryResolve(IDependencyResolver resolver, out TValue value);

        object IResolvable.Resolve(IDependencyResolver resolver) => Resolve(resolver);

        public bool TryResolve(IDependencyResolver resolver, out object value)
        {
            if (TryResolve(resolver, out TValue obj))
            {
                value = obj;
                return true;
            }

            value = null;
            return false;
        }
    }
}