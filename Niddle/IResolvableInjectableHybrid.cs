namespace Niddle
{
    public interface IResolvableInjectableHybrid<out TResolvable, out TInjectable> : IResolvableInjectable
        where TResolvable : IResolvable
        where TInjectable : IInjectable
    {
        TResolvable Resolvable { get; }
        TInjectable Injectable { get; }
    }

    public interface IResolvableInjectableHybrid<out TResolvable, out TInjectable, in TTarget, TValue> : IResolvableInjectableHybrid<TResolvable, TInjectable>, IResolvableInjectable<TTarget, TValue>
        where TResolvable : IResolvable<TValue>
        where TInjectable : IInjectable<TTarget, TValue>
    {
    }
}