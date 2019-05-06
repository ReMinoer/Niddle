namespace Niddle
{
    public interface IRejecterMethod<in TTarget, in TValue, out TReject> : IInjectableMethod<TTarget, TValue>, IRejecter<TTarget, TValue, TReject>
    {
    }
}