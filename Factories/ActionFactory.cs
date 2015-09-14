using System;

namespace Diese.Injection.Factories
{
    internal class ActionFactory<TIn> : FactoryBase
    {
        private readonly Action<TIn> _action;

        public ActionFactory(Action<TIn> action, object serviceKey, Substitution substitution)
            : base(action.GetType(), serviceKey, substitution)
        {
            _action = action;
        }

        public override object Get(IDependencyInjector injector)
        {
            return _action;
        }
    }
}