using System;

namespace Niddle
{
    public interface IInjectable
    {
        Type Type { get; }
    }

    public interface IInjectable<in TTarget, in TValue> : IInjectable
    {
        void Inject(TTarget target, TValue value);
    }
}