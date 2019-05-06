namespace Niddle
{
    public interface IDependencyFactory : IFactory
    {
        object Get(IDependencyResolver resolver);
    }
}