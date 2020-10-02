using System;

namespace Domain.Generic
{
    public abstract class BaseException : Exception
    {
        /// <summary>
        /// Error code that can be used for things like HTTP status code
        /// </summary>
        public virtual int Code { get; protected set; }

        /// <summary>
        /// Points to a key in Errors.{locale}.resx file
        /// </summary>
        public virtual string LocalizableMessage { get; protected set; }

        public BaseException(string message = "Base Exception", Exception innerException = null) : base (message, innerException)
        {

        }
    }
}
