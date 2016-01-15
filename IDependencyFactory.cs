namespace Diese.Injection
{
    public interface IDependencyFactory : IInjectionService
    {
        object Get(IDependencyInjector injector);
    }
}