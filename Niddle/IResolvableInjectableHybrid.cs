namespace Niddle
{
    public interface IResolvableInjectableHybrid<out TResolvable, out TInjectable> : IResolvableInjectable
        where TResolvable : IResolvable
        where TInjectable : IInjectable
    {
        TResolvable Resolvable { get; }
        TInjectable Injectable { get; }
    }

    public interface IResolvableInjectableHybrid<out TResolvable, out TInjectable, in TTarget, out TResolvableValue, in TInjectableValue> : IResolvableInjectableHybrid<TResolvable, TInjectable>, IResolvableInjectable<TTarget, TResolvableValue, TInjectableValue>
        where TResolvable : IResolvable<TResolvableValue>
        where TInjectable : IInjectable<TTarget, TInjectableValue>
        where TResolvableValue : TInjectableValue
    {
    }
}