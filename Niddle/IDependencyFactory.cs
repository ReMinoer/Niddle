namespace Niddle
{
    public interface IDependencyFactory : IInjectionService
    {
        object Get(IDependencyInjector injector);
    }
}