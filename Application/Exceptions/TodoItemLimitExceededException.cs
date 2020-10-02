using Domain.Generic;
using System;

namespace Application.Exceptions
{
    /// <summary>
    /// Thrown when a user's todo-item limit has been exceeded
    /// </summary>
    public class TodoItemLimitExceededException : BaseException
    {        
        public int Used { get; private set; }
        public int Limit { get; private set; }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="used">Number of todo-item used</param>
        /// <param name="limit">Todo-item limit</param>
        /// <param name="innerException"></param>
        public TodoItemLimitExceededException(int used, int limit, Exception innerException = null) : base("Todo Item Limit has been exceeded", innerException)
        {
            Used = used;
            Limit = limit;

            Code = 400;
            LocalizableMessage = "TodoItemLimitExceeded";
        }
    }
}
