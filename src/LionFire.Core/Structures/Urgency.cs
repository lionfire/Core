using LionFire.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LionFire
{
    // REVIEW
    /// <summary>
    /// Devise nomenclature:
    ///  - P1
    /// </summary>
    public enum Urgency
    {
        Unspecified = 0,

        /// <summary>
        /// P1
        /// </summary>
        [Code("1")]
        Realtime = 1 << 0,
        
        /// <summary>
        /// P2 Near-immediate: within 5 seconds
        /// </summary>
        [Code("2")]
        Immediate = 1 << 1,

        /// <summary>
        /// P3 15 seconds
        /// </summary>
        [Code("3")]
        Flash, 

        /// <summary>
        /// P4 30 seconds
        /// </summary>
        [Code("4")]
        Live ,

        /// <summary>
        /// P5 - One minute
        /// </summary>
        [Code("5")]
        Urgent ,

        /// <summary>
        /// P6
        /// </summary>
        [Code("6")]
        TwoMinutes,

        /// <summary>
        /// P7
        /// </summary>
        [Code("7")]
        FiveMinutes,

        /// <summary>
        /// P8
        /// </summary>
        [Code("8")]
        QuarterHour,

        /// <summary>
        /// P9
        /// </summary>
        [Code("9")]
        HalfHour,

        /// <summary>
        /// P10
        /// </summary>
        [Code("A")]
        Hour,

        /// <summary>
        /// P11
        /// </summary>
        [Code("B")]
        TwoHour,

        /// <summary>
        /// P12
        /// </summary>
        [Code("C")]
        HalfDay,

        /// <summary>
        /// P13
        /// </summary>
        [Code("D")]
        Day,

        /// <summary>
        /// P14
        /// </summary>
        [Code("E")]
        Week ,

        /// <summary>
        /// P15
        /// </summary>
        [Code("F")]
        Month,

        /// <summary>
        /// P16
        /// </summary>
        [Code("G")]
        Year,
    }
    public static class NotificationUrgencyExtensions
    {
        public static string ToCode(this Urgency urgency)
        {
            var attr = typeof(Urgency).GetField(urgency.ToString()).GetCustomAttribute<CodeAttribute>();
            return attr?.Code;
        }
    }
}
