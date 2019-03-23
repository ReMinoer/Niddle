using System;

namespace Niddle.Builders.Base
{
    public abstract class DependencyBuilderBase<TFactory, TBuilder, TLinkBuilder> : IDependencyBuilder<TFactory, TBuilder, TLinkBuilder>
        where TBuilder : IDependencyBuilder<TFactory, TBuilder, TLinkBuilder>
    {
        protected bool Singleton { get; set; }
        protected object Key { get; set; }
        protected bool IsOverridable { get; set; }

        protected Substitution Substitution => IsOverridable ? Substitution.Allowed : Substitution.Forbidden;

        protected abstract TBuilder Builder { get; }

        protected abstract TFactory Build();
        TFactory IDependencyBuilder<TFactory>.Build() => Build();

        public TBuilder AsSingleton()
        {
            Singleton = true;
            return Builder;
        }

        public TBuilder Keyed(object key)
        {
            Key = key;
            return Builder;
        }

        public TBuilder Overridable()
        {
            IsOverridable = true;
            return Builder;
        }

        public abstract TLinkBuilder LinkedTo(Type linkedType);

        public abstract class LinkDependencyBuilderBase<TRootBuilder> : ILinkDependencyBuilder<TFactory, TLinkBuilder>
            where TRootBuilder : TBuilder
        {
            protected Type LinkedType { get; set; }
            protected object LinkedKey { get; set; }

            protected readonly TRootBuilder RootBuilder;
            protected abstract TLinkBuilder Builder { get; }

            protected LinkDependencyBuilderBase(TRootBuilder rootBuilder, Type linkedType)
            {
                RootBuilder = rootBuilder;
                LinkedType = linkedType;
            }

            public TLinkBuilder Keyed(object key)
            {
                LinkedKey = key;
                return Builder;
            }

            public TLinkBuilder Overridable()
            {
                RootBuilder.Overridable();
                return Builder;
            }

            TFactory ILinkDependencyBuilder<TFactory>.AddTo(IDependencyRegistry dependencyRegistry) => AddTo(dependencyRegistry);
            protected abstract TFactory AddTo(IDependencyRegistry dependencyRegistry);
        }
    }
}