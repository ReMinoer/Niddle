using System;
using Niddle.Dependencies.Factories.Base;

namespace Niddle.Dependencies.Factories
{
    internal class ActionFactory<TIn> : DependencyFactoryBase
    {
        private readonly Action<TIn> _action;
        public override InstanceOrigin? InstanceOrigin => Niddle.InstanceOrigin.Registration;

        public ActionFactory(Action<TIn> action, object serviceKey, Substitution substitution)
            : base(action.GetType(), serviceKey, substitution)
        {
            _action = action;
        }

        public override object Get(IDependencyResolver resolver)
        {
            return _action;
        }
    }
}