namespace Niddle.Resolvables
{
    public class CastingResolvable<TValue> : IResolvable<TValue>
    {
        public IResolvable Resolvable { get; set; }

        public TValue Resolve(IDependencyResolver resolver)
        {
            return (TValue)Resolvable.Resolve(resolver);
        }

        public bool TryResolve(IDependencyResolver resolver, out TValue value)
        {
            if (Resolvable.TryResolve(resolver, out object obj))
            {
                value = (TValue)obj;
                return true;
            }

            value = default(TValue);
            return false;
        }

        object IResolvable.Resolve(IDependencyResolver resolver) => Resolvable.Resolve(resolver);
        public bool TryResolve(IDependencyResolver resolver, out object value) => Resolvable.TryResolve(resolver, out value);
    }
}