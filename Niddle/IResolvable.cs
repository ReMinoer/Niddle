namespace Niddle
{
    public interface IResolvable
    {
        object Resolve(IDependencyResolver resolver);
        bool TryResolve(IDependencyResolver resolver, out object value);
    }

    public interface IResolvable<out TValue> : IResolvable
    {
        new TValue Resolve(IDependencyResolver resolver);
        IOptional<TValue> TryResolve(IDependencyResolver resolver);
    }
}