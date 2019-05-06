namespace Niddle
{
    public interface IResolvableRejecter<in TTarget, TValue, out TReject> : IResolvableInjectable<TTarget, TValue>, IRejecter<TTarget, TValue, TReject>
    {
        TReject ResolveAndReject(IDependencyResolver resolver, TTarget target);
        IOptional<TReject> TryResolveAndReject(IDependencyResolver resolver, TTarget target);
    }
}