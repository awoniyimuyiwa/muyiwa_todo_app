using Domain.Core;
using Domain.Generic;
using System;

namespace Infrastructure.Data
{
    class DataNotSavedException : BaseException
    {
        public DataNotSavedException(Exception innerException = null) : base("An error occured while saving data", innerException)
        {
            Code = 500;
            LocalizableMessage = "DataNotSaved";
        }
    }
}
