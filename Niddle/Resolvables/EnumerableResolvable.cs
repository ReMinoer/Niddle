using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Niddle.Resolvables.Base;
using Niddle.Utils;

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

        public override IOptional<IEnumerable> TryResolve(IDependencyResolver resolver)
        {
            var parameters = new List<object>(_resolvables.Count);
            foreach (IResolvable resolvable in _resolvables)
            {
                if (resolvable.TryResolve(resolver, out object parameterValue))
                    parameters.Add(parameterValue);
                else
                    return Optional<IEnumerable>.NoValue;
            }
            
            return new Optional<IEnumerable>(parameters);
        }

        public IEnumerator<IResolvable> GetEnumerator() => _resolvables.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}