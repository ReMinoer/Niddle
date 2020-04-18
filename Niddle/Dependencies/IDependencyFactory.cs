namespace Niddle.Dependencies
{
    public interface IDependencyFactory : IFactory
    {
        object Get(IDependencyResolver resolver);
    }
}