using System;

namespace LionFire.Extensions
{
    public static class DateExtensions
    {
        public static DateTime GetWithoutTime(this DateTime date) => new DateTime(date.Year, date.Month, date.Day);
#if NET6_0_OR_GREATER
        public static DateOnly ToDateOnly(this DateTime date) => new DateOnly(date.Year, date.Month, date.Day);
#endif
    }
}
