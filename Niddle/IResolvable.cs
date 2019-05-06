namespace Niddle
{
    public interface IResolvable
    {
        object Resolve(IDependencyResolver resolver);
        bool TryResolve(IDependencyResolver resolver, out object value);
    }

    public interface IResolvable<TValue> : IResolvable
    {
        new TValue Resolve(IDependencyResolver resolver);
        bool TryResolve(IDependencyResolver resolver, out TValue value);
    }
}