namespace Niddle.Resolvables
{
    public class DefaultableResolvable : IResolvable
    {
        public IResolvable Resolvable { get; }
        public object DefaultValue { get; }

        public DefaultableResolvable(IResolvable resolvable, object defaultValue)
        {
            Resolvable = resolvable;
            DefaultValue = defaultValue;
        }

        public object Resolve(IDependencyResolver resolver)
        {
            TryResolve(resolver, out object parameterValue);
            return parameterValue;
        }

        public bool TryResolve(IDependencyResolver resolver, out object value)
        {
            value = Resolvable.TryResolve(resolver, out object parameterValue) ? parameterValue : DefaultValue;
            return true;
        }
    }
}