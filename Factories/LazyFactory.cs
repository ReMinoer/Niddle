using System;

namespace Diese.Injection.Factories
{
    internal class LazyFactory<T> : FactoryBase
    {
        private readonly Lazy<T> _lazy;

        public LazyFactory(Func<T> factory, object serviceKey, Substitution substitution)
            : base(typeof(Lazy<T>), serviceKey, substitution)
        {
            _lazy = new Lazy<T>(factory);
        }

        public override object Get(IDependencyInjector injector)
        {
            return _lazy;
        }
    }
}