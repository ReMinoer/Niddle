using System;
using Niddle.Factories.Base;

namespace Niddle.Factories
{
    internal class LazyFactory<T> : DependencyFactoryBase
    {
        private readonly Lazy<T> _lazy;
        public override InstanceOrigin? InstanceOrigin => Niddle.InstanceOrigin.Registration;

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