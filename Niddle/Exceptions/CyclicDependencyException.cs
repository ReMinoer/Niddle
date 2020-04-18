using System;
using System.Collections.Generic;
using System.Linq;
using Niddle.Dependencies;

namespace Niddle.Exceptions
{
    internal class CyclicDependencyException : Exception
    {
        private const string CustomMessage = "Cyclic dependency detected ! ({0})";

        public CyclicDependencyException(IEnumerable<IDependencyFactory> factoryStack)
            : base(string.Format(CustomMessage,
                string.Join(",", factoryStack.Select(x => x.Key != null ? "Key:" + x.Key : x.Type.Name))))
        {
        }
    }
}