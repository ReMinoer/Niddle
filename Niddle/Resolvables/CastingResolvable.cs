using Niddle.Utils;

namespace Niddle.Resolvables
{
    public class CastingResolvable<TValue> : IResolvable<TValue>
    {
        public IResolvable Resolvable { get; set; }

        public TValue Resolve(IDependencyResolver resolver)
        {
            return (TValue)Resolvable.Resolve(resolver);
        }

        public IOptional<TValue> TryResolve(IDependencyResolver resolver)
        {
            return Resolvable.TryResolve(resolver, out object obj)
                ? (TValue)obj
                : Optional<TValue>.NoValue;
        }

        object IResolvable.Resolve(IDependencyResolver resolver) => Resolvable.Resolve(resolver);
        public bool TryResolve(IDependencyResolver resolver, out object value) => Resolvable.TryResolve(resolver, out value);
    }
}