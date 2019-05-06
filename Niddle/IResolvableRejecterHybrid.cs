namespace Niddle
{
    public interface IResolvableRejecterHybrid<out TResolvable, out TInjectable, in TTarget, TValue, out TReject> : IResolvableInjectableHybrid<TResolvable, TInjectable, TTarget, TValue>, IResolvableRejecter<TTarget, TValue, TReject>
        where TResolvable : IResolvable<TValue>
        where TInjectable : IRejecter<TTarget, TValue, TReject>
    {
    }
}