using System;

namespace Niddle
{
    public interface IInjectionService
    {
        Type Type { get; }
        object ServiceKey { get; }
        Substitution Substitution { get; }
        InstanceOrigin? InstanceOrigin { get; }
    }
}