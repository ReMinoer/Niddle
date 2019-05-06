namespace Niddle.Utils
{
    public class Optional<T> : IOptional<T>
    {
        public bool HasValue { get; }
        public T Value { get; }
        
        private Optional() {}
        private Optional(T value)
        {
            HasValue = true;
            Value = value;
        }

        static public Optional<T> NoValue => new Optional<T>();
        static public implicit operator Optional<T>(T value)
        {
            return new Optional<T>(value);
        }
    }
}