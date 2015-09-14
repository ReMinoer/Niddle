using System;

namespace Diese.Injection.Exceptions
{
    public class AlreadyRegisterException : Exception
    {
        private const string TypeMessage = "Type \"{0}\" is already registered !";
        private const string KeyMessage = "Key \"{0}\" is already registered !";

        public AlreadyRegisterException(Type type)
            : base(string.Format(TypeMessage, type.Name))
        {
        }

        public AlreadyRegisterException(object key)
            : base(string.Format(KeyMessage, key))
        {
        }
    }
}