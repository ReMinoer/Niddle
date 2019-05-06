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

        new public TValue Resolve(IDependencyInjector injector) => Resolvable.Resolve(injector);
        public bool TryResolve(IDependencyInjector injector, out TValue value) => Resolvable.TryResolve(injector, out value);
        public void Inject(TTarget target, TValue value) => Injectable.Inject(target, value);
        public void ResolveAndInject(IDependencyInjector injector, TTarget target) => Inject(target, Resolve(injector));
        public bool TryResolveAndInject(IDependencyInjector injector, TTarget target)
        {
            if (!TryResolve(injector, out TValue value))
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

        public object Resolve(IDependencyInjector injector) => Resolvable.Resolve(injector);
        public bool TryResolve(IDependencyInjector injector, out object value) => Resolvable.TryResolve(injector, out value);
    }
}