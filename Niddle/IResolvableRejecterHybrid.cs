namespace Niddle
{
    public interface IResolvableRejecterHybrid<out TResolvable, out TInjectable, in TTarget, out TResolvableValue, in TInjectableValue, out TReject> : IResolvableInjectableHybrid<TResolvable, TInjectable, TTarget, TResolvableValue, TInjectableValue>, IResolvableRejecter<TTarget, TResolvableValue, TInjectableValue, TReject>
        where TResolvable : IResolvable<TResolvableValue>
        where TInjectable : IRejecter<TTarget, TInjectableValue, TReject>
        where TResolvableValue : TInjectableValue
    {
    }
}