using System;

namespace Niddle.Dependencies
{
    public interface IFactory
    {
        Type Type { get; }
        object Key { get; }
        Substitution Substitution { get; }
        InstanceOrigin? InstanceOrigin { get; }
    }
}