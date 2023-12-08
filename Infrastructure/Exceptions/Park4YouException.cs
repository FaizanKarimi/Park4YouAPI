using System;
using System.Runtime.Serialization;
using Infrastructure.Enums;

namespace Infrastructure.Exceptions
{
    [Serializable]
    public class Park4YouException : Exception
    {
        public Park4YouException()
        : base() { }

        public Park4YouException(ErrorMessages message)
            : base(message.ToString()) { }

        public Park4YouException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public Park4YouException(string message, Exception innerException)
            : base(message, innerException) { }

        public Park4YouException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        protected Park4YouException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}