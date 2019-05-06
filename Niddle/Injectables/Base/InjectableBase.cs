using System;
using Niddle.Injectables.Expressions;

namespace Niddle.Injectables.Base
{
    public class InjectableBase : IInjectable
    {
        public Type Type { get; }
        public IInjectionExpression InjectionExpression { get; }

        protected InjectableBase(IInjectionExpression injectionExpression, Type type)
        {
            InjectionExpression = injectionExpression;
            Type = type;
        }
    }
}