using Domain.Generic;
using System;

namespace Web.Exceptions
{
    /// <summary>
    /// Thrown when a date range string is not in the format: dd month yyyy - dd month yyyy
    /// </summary>
    internal class DateRangeValidationException : BaseException
    {
        /// <summary>
        /// The name of the field being validated
        /// </summary>
        internal string FieldName { get; private set; }

        /// <summary>
        /// Initializes fields
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="innerException"></param>
        internal DateRangeValidationException(string fieldName = "date_range", Exception innerException = null) : base($"The {fieldName} field must be in the format: dd month yyyy - dd month yyyy", innerException)
        {
            FieldName = fieldName;

            Code = 400;
            LocalizableMessage = "InvalidDateRangeFormat";
        }
    }
}
