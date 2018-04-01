using System;

namespace Niddle
{
    [Flags]
    public enum InstanceOrigins
    {
        None = 0,
        Instantiation = 1 << 0,
        Registration = 1 << 1,
        All = Instantiation | Registration
    }
}