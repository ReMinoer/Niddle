using System;

namespace Diese.Injection.Exceptions
{
    public class NotRegisterException : Exception
    {
        private const string TypeMessage = "Type \"{0}\" is not registered !";
        private const string KeyMessage = "Key \"{0}\" is not registered !";

        public NotRegisterException(Type type)
            : base(string.Format(TypeMessage, type.Name))
        {
        }

        public NotRegisterException(object key)
            : base(string.Format(KeyMessage, key))
        {
        }
    }
}