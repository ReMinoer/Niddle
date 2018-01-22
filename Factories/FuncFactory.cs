using System;
using Diese.Injection.Factories.Base;

namespace Diese.Injection.Factories
{
    internal class FuncFactory<TOut> : DependencyFactoryBase
    {
        private readonly Func<TOut> _func;
        public override InstanceOrigin? InstanceOrigin => Injection.InstanceOrigin.Registration;

        public FuncFactory(Func<TOut> func, object serviceKey, Substitution substitution)
            : base(func.GetType(), serviceKey, substitution)
        {
            _func = func;
        }

        public override object Get(IDependencyInjector injector)
        {
            return _func;
        }
    }

    internal class FuncFactory<TIn, TOut> : DependencyFactoryBase
    {
        private readonly Func<TIn, TOut> _func;
        public override InstanceOrigin? InstanceOrigin => Injection.InstanceOrigin.Registration;

        public FuncFactory(Func<TIn, TOut> func, object serviceKey, Substitution substitution)
            : base(func.GetType(), serviceKey, substitution)
        {
            _func = func;
        }

        public override object Get(IDependencyInjector injector)
        {
            return _func;
        }
    }
}