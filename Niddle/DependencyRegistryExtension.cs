using Niddle.Dependencies;
using Niddle.Dependencies.Builders;

namespace Niddle
{
    static public class DependencyRegistryExtension
    {
        static public void Add(this IDependencyRegistry dependencyRegistry, IDependencyBuilder<IDependencyFactory> typeDependencyBuilder)
        {
            dependencyRegistry.Add(typeDependencyBuilder.Build());
        }

        static public void Add(this IDependencyRegistry dependencyRegistry, IDependencyBuilder<IGenericFactory> genericDependencyBuilder)
        {
            dependencyRegistry.Add(genericDependencyBuilder.Build());
        }

        static public void Add(this IDependencyRegistry dependencyRegistry, ILinkDependencyBuilder<IDependencyFactory> typeDependencyBuilder)
        {
            typeDependencyBuilder.AddTo(dependencyRegistry);
        }

        static public void Add(this IDependencyRegistry dependencyRegistry, ILinkDependencyBuilder<IGenericFactory> genericDependencyBuilder)
        {
            genericDependencyBuilder.AddTo(dependencyRegistry);
        }
    }
}