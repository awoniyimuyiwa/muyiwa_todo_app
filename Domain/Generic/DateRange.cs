using System;

namespace Domain.Generic
{
    /// <summary>
    /// Represents a Date with min and max DateTime
    /// </summary>
    public class DateRange
    {
        public readonly DateTime Min;
        public readonly DateTime Max;
        static readonly string DateTimeFormat = "dd MMMM yyyy";

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="min">The minimum DateTime</param>
        /// <param name="max">The maximum DateTime</param>
        public DateRange(DateTime min, DateTime max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Parses a date range string in the format dd MMMM yyyy-dd MMMM yyyy to a DateRange object
        /// </summary>
        /// <param name="dateRangeString">String in the format: dd MMMM yyyy-dd MMMM yyyy</param>
        /// <param name="setDefaults">Set to true if a default DateRange object should be returned when <paramref name="dateRangeString"/> is null</param>
        /// <returns>DateRange</returns>
        /// <exception cref="System.Exception">Thrown when <paramref name="dateRangeString"/> is not in the right format</exception>
        public static DateRange Parse(string dateRangeString = null, bool setDefaults = false)
        {
            if (string.IsNullOrEmpty(dateRangeString))
            {
                if (!setDefaults) { return null; }

                var today = DateTime.Today;
                // Start of current month
                var min = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                // End of current month
                var max = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month), 23, 59, 59, DateTimeKind.Utc);

                return new DateRange(min, max);
            }

            try
            {
                var dateArray = dateRangeString.Split("-");
                var min = DateTime.ParseExact(dateArray[0], DateTimeFormat, System.Globalization.CultureInfo.InvariantCulture);
                var max = DateTime.ParseExact(dateArray[1], DateTimeFormat, System.Globalization.CultureInfo.InvariantCulture);

                // Start of min
                min = new DateTime(min.Year, min.Month, min.Day, 0, 0, 0, DateTimeKind.Utc);
                // End of max
                max = new DateTime(max.Year, max.Month, max.Day, 23, 59, 59, DateTimeKind.Utc);

                return new DateRange(min, max);
            }
            catch (Exception ex) { throw ex; }
        }

        public override string ToString()
        {
            return $"{Min:dd MMMM yyyy}-{Max:dd MMMM yyyy}";
        }
    }
}
