namespace Niddle.Resolvables
{
    public class CastingResolvable<TValue> : IResolvable<TValue>
    {
        public IResolvable Resolvable { get; set; }

        public TValue Resolve(IDependencyInjector injector)
        {
            return (TValue)Resolvable.Resolve(injector);
        }

        public bool TryResolve(IDependencyInjector injector, out TValue value)
        {
            if (Resolvable.TryResolve(injector, out object obj))
            {
                value = (TValue)obj;
                return true;
            }

            value = default(TValue);
            return false;
        }

        object IResolvable.Resolve(IDependencyInjector injector) => Resolvable.Resolve(injector);
        public bool TryResolve(IDependencyInjector injector, out object value) => Resolvable.TryResolve(injector, out value);
    }
}