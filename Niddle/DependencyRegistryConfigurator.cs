using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Niddle.Builders;

namespace Niddle
{
    public class DependencyRegistryConfigurator : IEnumerable
    {
        private readonly ICollection<IDependencyBuilder<IDependencyFactory>> _typeDependencyBuilders = new List<IDependencyBuilder<IDependencyFactory>>();
        private readonly ICollection<IDependencyBuilder<IGenericFactory>> _genericDependencyBuilders = new List<IDependencyBuilder<IGenericFactory>>();
        private readonly ICollection<ILinkDependencyBuilder<IDependencyFactory>> _linkTypeDependencyBuilders = new List<ILinkDependencyBuilder<IDependencyFactory>>();
        private readonly ICollection<ILinkDependencyBuilder<IGenericFactory>> _linkGenericDependencyBuilders = new List<ILinkDependencyBuilder<IGenericFactory>>();

        public void Add(IDependencyBuilder<IDependencyFactory> typeDependencyBuilder)
        {
            _typeDependencyBuilders.Add(typeDependencyBuilder);
        }

        public void Add(IDependencyBuilder<IGenericFactory> genericDependencyBuilder)
        {
            _genericDependencyBuilders.Add(genericDependencyBuilder);
        }

        public void Add(ILinkDependencyBuilder<IDependencyFactory> linkTypeDependencyBuilder)
        {
            _linkTypeDependencyBuilders.Add(linkTypeDependencyBuilder);
        }

        public void Add(ILinkGenericDependencyBuilder linkGenericDependencyBuilder)
        {
            _linkGenericDependencyBuilders.Add(linkGenericDependencyBuilder);
        }

        public void Configure(IDependencyRegistry registry)
        {
            foreach (IDependencyBuilder<IDependencyFactory> typeDependencyBuilder in _typeDependencyBuilders)
                registry.Add(typeDependencyBuilder);

            foreach (IDependencyBuilder<IGenericFactory> genericDependencyBuilder in _genericDependencyBuilders)
                registry.Add(genericDependencyBuilder);

            foreach (ILinkDependencyBuilder<IDependencyFactory> linkTypeDependencyBuilder in _linkTypeDependencyBuilders)
                registry.Add(linkTypeDependencyBuilder);

            foreach (ILinkDependencyBuilder<IGenericFactory> linkGenericDependencyBuilder in _linkGenericDependencyBuilders)
                registry.Add(linkGenericDependencyBuilder);
        }

        public IEnumerator GetEnumerator()
        {
            return _typeDependencyBuilders
                   .Concat<object>(_genericDependencyBuilders)
                   .Concat(_linkTypeDependencyBuilders)
                   .Concat(_linkGenericDependencyBuilders)
                   .GetEnumerator();
        }
    }
}