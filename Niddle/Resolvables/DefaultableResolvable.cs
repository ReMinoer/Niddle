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

        public object Resolve(IDependencyInjector injector)
        {
            TryResolve(injector, out object parameterValue);
            return parameterValue;
        }

        public bool TryResolve(IDependencyInjector injector, out object value)
        {
            value = Resolvable.TryResolve(injector, out object parameterValue) ? parameterValue : DefaultValue;
            return true;
        }
    }
}