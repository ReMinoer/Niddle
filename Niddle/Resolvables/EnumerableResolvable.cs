using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Niddle.Resolvables.Base;

namespace Niddle.Resolvables
{
    public class EnumerableResolvable : ResolvableBase<IEnumerable>, IEnumerable<IResolvable>
    {
        private readonly ICollection<IResolvable> _resolvables;

        public EnumerableResolvable(IEnumerable<IResolvable> resolvables)
        {
            _resolvables = resolvables.ToArray();
        }

        public override IEnumerable Resolve(IDependencyResolver resolver)
        {
            return _resolvables.Select(x => x.Resolve(resolver));
        }

        public override bool TryResolve(IDependencyResolver resolver, out IEnumerable value)
        {
            var parameters = new List<object>(_resolvables.Count);
            foreach (IResolvable resolvable in _resolvables)
            {
                if (resolvable.TryResolve(resolver, out object parameterValue))
                {
                    parameters.Add(parameterValue);
                }
                else
                {
                    value = null;
                    return false;
                }
            }

            value = parameters;
            return true;
        }

        public IEnumerator<IResolvable> GetEnumerator() => _resolvables.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}