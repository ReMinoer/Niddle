using System;
using System.Reflection;
using Niddle.Dependencies.Builders.Base;
using Niddle.Dependencies.Builders.Helpers;
using Niddle.Dependencies.Factories;

namespace Niddle.Dependencies.Builders
{
    public class GenericDependencyBuilder : DependencyBuilderBase<IGenericFactory, IGenericDependencyBuilder, ILinkGenericDependencyBuilder>, IGenericDependencyBuilder
    {
        protected Type TypeDefinition { get; }
        protected Type ImplementationTypeDefinition { get; set; }
        protected ConstructorInfo Constructor { get; set; }

        protected InstanceOrigin Origin => Singleton ? InstanceOrigin.Registration : InstanceOrigin.Instantiation;

        protected override IGenericDependencyBuilder Builder => this;

        public GenericDependencyBuilder(Type typeDefinition)
        {
            TypeDefinition = typeDefinition;
        }

        public IGenericDependencyBuilder Creating(Type implementationTypeDefinition)
        {
            ImplementationTypeDefinition = implementationTypeDefinition;
            return Builder;
        }

        public IGenericDependencyBuilder Creating(Type implementationTypeDefinition, ConstructorInfo constructorInfo)
        {
            ImplementationTypeDefinition = implementationTypeDefinition;
            Constructor = constructorInfo;
            return Builder;
        }

        protected override IGenericFactory Build()
        {
            ConstructorInfo constructor = Constructor ?? ReflectionHelper.GetDefaultConstructor(ImplementationTypeDefinition ?? TypeDefinition);
            return new GenericFactory(TypeDefinition, ImplementationTypeDefinition, Origin, Key, constructor, Substitution, ResolvableMembersProvider);
        }

        public override ILinkGenericDependencyBuilder LinkedTo(Type linkedType)
        {
            return new LinkBuilder(this, linkedType);
        }

        public class LinkBuilder : LinkDependencyBuilderBase<GenericDependencyBuilder>, ILinkGenericDependencyBuilder
        {
            protected override ILinkGenericDependencyBuilder Builder => this;

            public LinkBuilder(GenericDependencyBuilder rootBuilder, Type linkedType)
                : base(rootBuilder, linkedType)
            {
            }
            
            protected override IGenericFactory AddTo(IDependencyRegistry dependencyRegistry)
            {
                var linkedGenericFactory = new LinkedGenericFactory(dependencyRegistry, RootBuilder.TypeDefinition, LinkedType, LinkedKey, RootBuilder.Key, RootBuilder.Substitution);
                dependencyRegistry.Add(linkedGenericFactory);
                return linkedGenericFactory;
            }
        }
    }
}