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

    public interface IResolvableInjectable<in TTarget, TValue> : IResolvableInjectable<TTarget>, IResolvable<TValue>, IInjectable<TTarget, TValue>
    {
    }
}