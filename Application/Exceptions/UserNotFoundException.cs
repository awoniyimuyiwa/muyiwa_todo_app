using Domain.Generic;
using System;

namespace Application.Exceptions
{
    /// <summary>
    /// Thrown when a user is not found
    /// </summary>
    public class UserNotFoundException : BaseException
    {        
        /// <summary>
        /// Initializes fields/properties
        /// </summary>
        public UserNotFoundException(Exception innerException = null) : base("User not found", innerException)
        {
            Code = 400;
            LocalizableMessage = "UserNotFound";
        }
    }
}
