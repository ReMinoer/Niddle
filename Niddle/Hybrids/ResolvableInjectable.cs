using System;

namespace Niddle.Hybrids
{
    public class ResolvableInjectable<TResolvable, TInjectable, TTarget, TResolvableValue, TInjectableValue> : ResolvableInjectable<TResolvable, TInjectable>, IResolvableInjectableHybrid<TResolvable, TInjectable, TTarget, TResolvableValue, TInjectableValue>
        where TResolvable : IResolvable<TResolvableValue>
        where TInjectable : IInjectable<TTarget, TInjectableValue>
        where TResolvableValue : TInjectableValue
    {
        public ResolvableInjectable(TResolvable resolvable, TInjectable injectable)
            : base(resolvable, injectable)
        {
        }

        new public TResolvableValue Resolve(IDependencyResolver resolver) => Resolvable.Resolve(resolver);
        public IOptional<TResolvableValue> TryResolve(IDependencyResolver resolver) => Resolvable.TryResolve(resolver);
        public void Inject(TTarget target, TInjectableValue value) => Injectable.Inject(target, value);
        public void ResolveAndInject(IDependencyResolver resolver, TTarget target) => Inject(target, Resolve(resolver));
        public bool TryResolveAndInject(IDependencyResolver resolver, TTarget target)
        {
            IOptional<TResolvableValue> resolvedValue = TryResolve(resolver);
            if (!resolvedValue.HasValue)
                return false;

            Inject(target, resolvedValue.Value);
            return true;
        }
    }

    public class ResolvableInjectable<TResolvable, TInjectable> : IResolvableInjectableHybrid<TResolvable, TInjectable>
        where TResolvable : IResolvable
        where TInjectable : IInjectable
    {
        public TResolvable Resolvable { get; }
        public TInjectable Injectable { get; }
        
        public Type Type => Injectable.Type;

        public ResolvableInjectable(TResolvable resolvable, TInjectable injectable)
        {
            Resolvable = resolvable;
            Injectable = injectable;
        }

        public object Resolve(IDependencyResolver resolver) => Resolvable.Resolve(resolver);
        public bool TryResolve(IDependencyResolver resolver, out object value) => Resolvable.TryResolve(resolver, out value);
    }
}