using System;

namespace Niddle.Exceptions
{
    public class NotRegisterException : Exception
    {
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