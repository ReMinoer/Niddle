using Niddle.Utils;

namespace Niddle.Hybrids
{
    public class ResolvableRejecter<TResolvable, TInjectable, TTarget, TValue, TReject> : ResolvableInjectable<TResolvable, TInjectable, TTarget, TValue>, IResolvableRejecterHybrid<TResolvable, TInjectable, TTarget, TValue, TReject>
        where TResolvable : IResolvable<TValue>
        where TInjectable : IRejecter<TTarget, TValue, TReject>
    {
        public ResolvableRejecter(TResolvable resolvable, TInjectable injectable)
            : base(resolvable, injectable)
        {
        }

        public TReject Reject(TTarget target, TValue value) => Injectable.Reject(target, value);
        public TReject ResolveAndReject(IDependencyInjector injector, TTarget target) => Reject(target, Resolve(injector));
        public IOptional<TReject> TryResolveAndReject(IDependencyInjector injector, TTarget target)
        {
            return TryResolve(injector, out TValue value)
                ? Reject(target, value)
                : Optional<TReject>.NoValue;
        }
    }
}