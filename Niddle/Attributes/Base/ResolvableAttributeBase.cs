using System;
using Niddle.Injectables.Expressions;

namespace Niddle.Attributes.Base
{
    public abstract class ResolvableAttributeBase : Attribute
    {
        public abstract IInjectionExpression GetInjectionScenario(Type memberType);
        public abstract IResolvable GetResolvable(Type memberType, Attribute[] attributes);
        public abstract IResolvable<TValue> GetResolvable<TValue>(Type memberType, Attribute[] attributes);
    }
}