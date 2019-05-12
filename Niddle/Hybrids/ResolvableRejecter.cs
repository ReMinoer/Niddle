using Niddle.Utils;

namespace Niddle.Hybrids
{
    public class ResolvableRejecter<TResolvable, TInjectable, TTarget, TResolvableValue, TInjectableValue, TReject> : ResolvableInjectable<TResolvable, TInjectable, TTarget, TResolvableValue, TInjectableValue>, IResolvableRejecterHybrid<TResolvable, TInjectable, TTarget, TResolvableValue, TInjectableValue, TReject>
        where TResolvable : IResolvable<TResolvableValue>
        where TInjectable : IRejecter<TTarget, TInjectableValue, TReject>
        where TResolvableValue : TInjectableValue
    {
        public ResolvableRejecter(TResolvable resolvable, TInjectable injectable)
            : base(resolvable, injectable)
        {
        }

        public TReject Reject(TTarget target, TInjectableValue value) => Injectable.Reject(target, value);
        public TReject ResolveAndReject(IDependencyResolver resolver, TTarget target) => Reject(target, Resolve(resolver));
        public IOptional<TReject> TryResolveAndReject(IDependencyResolver resolver, TTarget target)
        {
            IOptional<TResolvableValue> resolvedValue = TryResolve(resolver);
            return resolvedValue.HasValue
                ? Reject(target, resolvedValue.Value)
                : Optional<TReject>.NoValue;
        }
    }
}