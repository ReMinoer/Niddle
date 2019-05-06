namespace Niddle
{
    public interface IRejecter<in TTarget, in TValue, out TReject> : IInjectable<TTarget, TValue>
    {
        TReject Reject(TTarget target, TValue value);
    }
}