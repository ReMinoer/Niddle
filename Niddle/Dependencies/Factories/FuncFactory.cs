using System;
using Niddle.Dependencies.Factories.Base;

namespace Niddle.Dependencies.Factories
{
    internal class FuncFactory<TOut> : DependencyFactoryBase
    {
        private readonly Func<TOut> _func;
        public override InstanceOrigin? InstanceOrigin => Niddle.InstanceOrigin.Registration;

        public FuncFactory(Func<TOut> func, object serviceKey, Substitution substitution)
            : base(func.GetType(), serviceKey, substitution)
        {
            _func = func;
        }

        public override object Get(IDependencyResolver resolver)
        {
            return _func;
        }
    }

    internal class FuncFactory<TIn, TOut> : DependencyFactoryBase
    {
        private readonly Func<TIn, TOut> _func;
        public override InstanceOrigin? InstanceOrigin => Niddle.InstanceOrigin.Registration;

        public FuncFactory(Func<TIn, TOut> func, object serviceKey, Substitution substitution)
            : base(func.GetType(), serviceKey, substitution)
        {
            _func = func;
        }

        public override object Get(IDependencyResolver resolver)
        {
            return _func;
        }
    }
}