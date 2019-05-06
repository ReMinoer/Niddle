namespace Niddle
{
    public interface IResolvableInjectable : IResolvable, IInjectable
    {
    }

    public interface IResolvableInjectable<in TTarget> : IResolvableInjectable
    {
        void ResolveAndInject(IDependencyInjector injector, TTarget target);
        bool TryResolveAndInject(IDependencyInjector injector, TTarget target);
    }

    public interface IResolvableInjectable<in TTarget, TValue> : IResolvableInjectable<TTarget>, IResolvable<TValue>, IInjectable<TTarget, TValue>
    {
    }
}