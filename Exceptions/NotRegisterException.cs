using System;

namespace Diese.Injection.Exceptions
{
    public class NotRegisterException : Exception
    {
        private const string TypeMessage = "Type \"{0}\" is not registered !";
        private const string KeyMessage = "Type \"{0}\" keyed \"{1}\" is not registered !";

        public NotRegisterException(Type type)
            : base(string.Format(TypeMessage, type.Name))
        {
        }

        public NotRegisterException(Type type, object key)
            : base(string.Format(KeyMessage, type, key))
        {
        }

        protected NotRegisterException(string message)
            : base(message)
        {
        }
    }
}