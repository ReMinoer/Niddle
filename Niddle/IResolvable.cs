namespace Niddle
{
    public interface IResolvable
    {
        object Resolve(IDependencyInjector injector);
        bool TryResolve(IDependencyInjector injector, out object value);
    }

    public interface IResolvable<TValue> : IResolvable
    {
        new TValue Resolve(IDependencyInjector injector);
        bool TryResolve(IDependencyInjector injector, out TValue value);
    }
}