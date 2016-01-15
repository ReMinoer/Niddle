namespace Diese.Injection
{
    public interface IDependencyFactory : IServiceFactory
    {
        object Get(IDependencyInjector injector);
    }
}