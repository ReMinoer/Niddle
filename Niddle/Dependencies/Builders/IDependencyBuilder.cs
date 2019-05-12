using System;
using System.Reflection;

namespace Niddle.Dependencies.Builders
{
    public interface IDependencyBuilder<out TFactory>
    {
        TFactory Build();
    }

    public interface IDependencyBuilder<out TFactory, out TBuilder, out TLinkBuilder> : IDependencyBuilder<TFactory>
    {
        TBuilder AsSingleton();
        TBuilder Keyed(object key);
        TBuilder Overridable();
        TBuilder ResolvingMembersFrom(IResolvableMembersProvider<object> provider);
        TLinkBuilder LinkedTo(Type linkedType);
    }

    public interface ITypeDependencyBuilder<in T, out TBuilder, out TLinkBuilder> : IDependencyBuilder<IDependencyFactory, TBuilder, TLinkBuilder>
    {
        TBuilder Using(T instance);
        TBuilder Creating<TImplementation>() where TImplementation : T;
        TBuilder Creating(Type implementationType);
        TBuilder Creating(ConstructorInfo constructorInfo);
        TBuilder Creating(MethodInfo methodInfo);
        TBuilder Creating<TImplementation>(Func<TImplementation> func) where TImplementation : T;
        TLinkBuilder LinkedTo<TLinked>() where TLinked : T;
    }

    public interface ITypeDependencyBuilder<in T> : ITypeDependencyBuilder<T, ITypeDependencyBuilder<T>, ILinkTypeDependencyBuilder>
    {
    }

    public interface IGenericDependencyBuilder<out TBuilder, out TLinkBuilder> : IDependencyBuilder<IGenericFactory, TBuilder, TLinkBuilder>
    {
        TBuilder Creating(Type implementationTypeDefinition);
        TBuilder Creating(Type implementationTypeDefinition, ConstructorInfo constructorInfo);
    }

    public interface IGenericDependencyBuilder : IGenericDependencyBuilder<IGenericDependencyBuilder, ILinkGenericDependencyBuilder>
    {
    }

    public interface ILinkDependencyBuilder<out TFactory>
    {
        TFactory AddTo(IDependencyRegistry dependencyRegistry);
    }

    public interface ILinkDependencyBuilder<out TFactory, out TBuilder> : ILinkDependencyBuilder<TFactory>
    {
        TBuilder Keyed(object key);
        TBuilder Overridable();
    }

    public interface ILinkTypeDependencyBuilder<out TBuilder> : ILinkDependencyBuilder<IDependencyFactory, TBuilder>
    {
    }

    public interface ILinkTypeDependencyBuilder : ILinkTypeDependencyBuilder<ILinkTypeDependencyBuilder>
    {
    }

    public interface ILinkGenericDependencyBuilder<out TBuilder> : ILinkDependencyBuilder<IGenericFactory, TBuilder>
    {
    }

    public interface ILinkGenericDependencyBuilder : ILinkGenericDependencyBuilder<ILinkGenericDependencyBuilder>
    {
    }
}