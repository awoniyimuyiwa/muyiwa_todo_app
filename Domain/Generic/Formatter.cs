using System;

namespace Domain.Generic
{
    public static class Formatter
    {
        public static string Format(DateTime dateTime)
        {
            return dateTime.ToString("dd-MM-yyyy HH:mm:ss");
        }
    }
}
