using System;
using System.Collections.Generic;

namespace Niddle
{
    public interface IDependencyRegistry : IEnumerable<IFactory>
    {
        IDependencyFactory this[Type type, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All] { get; }
        IDependencyFactory this[Type genericTypeDefinition, IEnumerable<Type> genericTypeArguments, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All] { get; }
        bool TryGetFactory(out IDependencyFactory factory, Type type, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All);
        bool TryGetFactory(out IDependencyFactory factory, Type genericTypeDefinition, IEnumerable<Type> genericTypeArguments, object serviceKey = null, InstanceOrigins instanceOrigins = InstanceOrigins.All);

        void Add(IDependencyFactory dependencyFactory);
        void Add(IGenericFactory genericFactory);
    }
}