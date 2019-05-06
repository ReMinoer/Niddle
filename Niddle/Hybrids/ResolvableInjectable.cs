using System;

namespace Niddle.Hybrids
{
    public class ResolvableInjectable<TResolvable, TInjectable, TTarget, TValue> : ResolvableInjectable<TResolvable, TInjectable>, IResolvableInjectableHybrid<TResolvable, TInjectable, TTarget, TValue>
        where TResolvable : IResolvable<TValue>
        where TInjectable : IInjectable<TTarget, TValue>
    {
        public ResolvableInjectable(TResolvable resolvable, TInjectable injectable)
            : base(resolvable, injectable)
        {
        }

        new public TValue Resolve(IDependencyResolver resolver) => Resolvable.Resolve(resolver);
        public bool TryResolve(IDependencyResolver resolver, out TValue value) => Resolvable.TryResolve(resolver, out value);
        public void Inject(TTarget target, TValue value) => Injectable.Inject(target, value);
        public void ResolveAndInject(IDependencyResolver resolver, TTarget target) => Inject(target, Resolve(resolver));
        public bool TryResolveAndInject(IDependencyResolver resolver, TTarget target)
        {
            if (!TryResolve(resolver, out TValue value))
                return false;

            Inject(target, value);
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