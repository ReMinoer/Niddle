namespace Niddle
{
    public interface IResolvableRejecter<in TTarget, out TResolvableValue, in TInjectableValue, out TReject> : IResolvableInjectable<TTarget, TResolvableValue, TInjectableValue>, IRejecter<TTarget, TInjectableValue, TReject>
        where TResolvableValue : TInjectableValue
    {
        TReject ResolveAndReject(IDependencyResolver resolver, TTarget target);
        IOptional<TReject> TryResolveAndReject(IDependencyResolver resolver, TTarget target);
    }
}