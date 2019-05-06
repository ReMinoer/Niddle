using System.Collections.Generic;
using Niddle.Injectables;

namespace Niddle
{
    public interface IInjectableMethod<in TTarget, in TValue> : IInjectable<TTarget, TValue>
    {
        IReadOnlyCollection<InjectableParameter> Parameters { get; }
    }
}