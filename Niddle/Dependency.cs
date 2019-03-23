using System;
using Niddle.Builders;

namespace Niddle
{
    static public class Dependency
    {
        static public TypeDependencyBuilder<object> OnType(Type type) => new TypeDependencyBuilder<object>(type);
        static public TypeDependencyBuilder<T> OnType<T>() => new TypeDependencyBuilder<T>(typeof(T));
        static public GenericDependencyBuilder OnGeneric(Type typeDefinition) => new GenericDependencyBuilder(typeDefinition);
    }
}