﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire
{
    public static class DateTimeUtils
    {
        public static DateTime RoundMillisecondsToZero(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Kind);
        }
        public static DateTime RoundSecondsToZero(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0, value.Kind);
        }
        public static DateTime RoundMinutesToZero(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, value.Hour, 0, 0, value.Kind);
        }
        public static DateTime RoundHoursToZero(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 0, 0, 0, value.Kind);
        }
        public static DateTime RoundDaysToOne(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1, 0, 0, 0, value.Kind);
        }
        public static DateTime RoundMonthsToOne(this DateTime value)
        {
            return new DateTime(value.Year, 1, 1, 0, 0, 0, value.Kind);
        }

        public static bool IsSameMinute(this DateTime time1, DateTime time2)
        {
            return time1.Year == time2.Year
                && time1.Month == time2.Month
                && time1.Day == time2.Day
                && time1.Hour == time2.Hour
                && time1.Minute == time2.Minute;
        }
    }
}
