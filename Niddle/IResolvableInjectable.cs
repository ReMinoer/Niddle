namespace Niddle
{
    public interface IResolvableInjectable : IResolvable, IInjectable
    {
    }

    public interface IResolvableInjectable<in TTarget> : IResolvableInjectable
    {
        void ResolveAndInject(IDependencyResolver resolver, TTarget target);
        bool TryResolveAndInject(IDependencyResolver resolver, TTarget target);
    }

    public interface IResolvableInjectable<in TTarget, out TResolvableValue, in TInjectableValue> : IResolvableInjectable<TTarget>, IResolvable<TResolvableValue>, IInjectable<TTarget, TInjectableValue>
        where TResolvableValue : TInjectableValue
    {
    }
}