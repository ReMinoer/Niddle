using System;

namespace Diese.Injection.Factories
{
    internal class FuncFactory<TOut> : FactoryBase
    {
        private readonly Func<TOut> _func;

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

    internal class FuncFactory<TIn, TOut> : FactoryBase
    {
        private readonly Func<TIn, TOut> _func;

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