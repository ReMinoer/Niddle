using System;
using System.Collections.Generic;
using System.Linq;
using Diese.Injection.Factories;

namespace Diese.Injection.Exceptions
{
    internal class CyclicDependencyException : Exception
    {
        private const string CustomMessage = "Cyclic dependency detected ! ({0})";

        public CyclicDependencyException(IEnumerable<IDependencyFactory> factoryStack)
            : base(string.Format(CustomMessage,
            string.Join(",", factoryStack.Select(x => x.ServiceKey != null ? "Key:" + x.ServiceKey : x.Type.Name))))
        {
        }
    }
}