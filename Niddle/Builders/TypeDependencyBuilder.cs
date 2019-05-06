using System;
using System.Collections;
using System.Reflection;
using Niddle.Builders.Base;
using Niddle.Builders.Helpers;
using Niddle.Factories;

namespace Niddle.Builders
{
    public class TypeDependencyBuilder<T> : DependencyBuilderBase<IDependencyFactory, ITypeDependencyBuilder<T>, ILinkTypeDependencyBuilder>, ITypeDependencyBuilder<T>
    {
        protected Type Type { get; }
        protected object Instance { get; set; }
        protected IResolvableRejecter<object, IEnumerable, object> Instantiator { get; set; }

        protected override ITypeDependencyBuilder<T> Builder => this;

        public TypeDependencyBuilder(Type type)
        {
            Type = type;
        }

        public ITypeDependencyBuilder<T> Using(T instance)
        {
            Instance = instance;
            return Builder;
        }

        public ITypeDependencyBuilder<T> Creating(Type implementationType)
        {
            return Creating(ReflectionHelper.GetDefaultConstructor(implementationType));
        }

        public ITypeDependencyBuilder<T> Creating(ConstructorInfo constructorInfo)
        {
            Instantiator = constructorInfo.AsResolvableRejecter();
            return Builder;
        }

        public ITypeDependencyBuilder<T> Creating(MethodInfo methodInfo)
        {
            Instantiator = methodInfo.AsResolvableRejecter();
            return Builder;
        }

        public ITypeDependencyBuilder<T> Creating<TImplementation>()
            where TImplementation : T
        {
            return Creating(typeof(TImplementation));
        }

        public ITypeDependencyBuilder<T> Creating<TImplementation>(Func<TImplementation> func)
            where TImplementation : T
        {
            return Creating(func.GetMethodInfo());
        }

        protected override IDependencyFactory Build()
        {
            if (Instance != null)
                return new InstanceFactory(Type, Instance, Key, Substitution);
            
            IResolvableRejecter<object, IEnumerable, object> instantiator =
                Instantiator
                ?? ReflectionHelper.GetDefaultConstructor(Type).AsResolvableRejecter<object>();

            return Singleton
                ? new SingletonFactory(Type, Key, instantiator, Substitution)
                : new NewInstanceFactory(Type, Key, instantiator, Substitution);
        }
        
        public ILinkTypeDependencyBuilder LinkedTo<TLinked>()
            where TLinked : T
        {
            return LinkedTo(typeof(TLinked));
        }

        public override ILinkTypeDependencyBuilder LinkedTo(Type linkedType)
        {
            return new LinkBuilder(this, linkedType);
        }

        public class LinkBuilder : LinkDependencyBuilderBase<TypeDependencyBuilder<T>>, ILinkTypeDependencyBuilder
        {
            protected override ILinkTypeDependencyBuilder Builder => this;

            public LinkBuilder(TypeDependencyBuilder<T> rootBuilder, Type linkedType)
                : base(rootBuilder, linkedType)
            {
            }
            
            protected override IDependencyFactory AddTo(IDependencyRegistry dependencyRegistry)
            {
                var dependencyFactory = new LinkedFactory(dependencyRegistry, RootBuilder.Type, LinkedType, LinkedKey, RootBuilder.Key, RootBuilder.Substitution);
                dependencyRegistry.Add(dependencyFactory);
                return dependencyFactory;
            }
        }
    }
}