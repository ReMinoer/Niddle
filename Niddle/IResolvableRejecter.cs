namespace Niddle
{
    public interface IResolvableRejecter<in TTarget, TValue, out TReject> : IResolvableInjectable<TTarget, TValue>, IRejecter<TTarget, TValue, TReject>
    {
        TReject ResolveAndReject(IDependencyInjector injector, TTarget target);
        IOptional<TReject> TryResolveAndReject(IDependencyInjector injector, TTarget target);
    }
}