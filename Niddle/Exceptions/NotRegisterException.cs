using System;

namespace Niddle.Exceptions
{
    public class NotRegisterException : Exception
    {
        private const string TypeMessage = "Type \"{0}\" is not registered !";
        private const string KeyMessage = "Type \"{0}\" keyed \"{1}\" is not registered !";

        public NotRegisterException(Type type, object key)
            : base($"Type \"{type.Name}\"{(key != null ? $" keyed \"{key}\"" : "")} is not registered !")
        {
        }

        protected NotRegisterException(string message)
            : base(message)
        {
        }
    }
}