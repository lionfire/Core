using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Notifications
{
    // REVIEW
    /// <summary>
    /// Devise nomenclature:
    ///  - P1
    /// </summary>
    public enum NotificationUrgency
    {
        Unspecified = 0,

        /// <summary>
        /// P1
        /// </summary>
        Realtime = 1 << 0,
        
        /// <summary>
        /// P2 Near-immediate: within 5 seconds
        /// </summary>
        Immediate = 1 << 1,

        /// <summary>
        /// P3 15 seconds
        /// </summary>
        Flash, 

        /// <summary>
        /// P4 30 seconds
        /// </summary>
        Live ,

        /// <summary>
        /// P5 - One minute
        /// </summary>
        Urgent ,

        /// <summary>
        /// P6
        /// </summary>
        TwoMinutes,

        /// <summary>
        /// P7
        /// </summary>
        FiveMinutes,

        /// <summary>
        /// P8
        /// </summary>
        QuarterHour,

        /// <summary>
        /// P9
        /// </summary>
        HalfHour,

        /// <summary>
        /// P10
        /// </summary>
        Hour,

        /// <summary>
        /// P11
        /// </summary>
        TwoHour,

        /// <summary>
        /// P12
        /// </summary>
        HalfDay,

        /// <summary>
        /// P13
        /// </summary>
        Day,

        /// <summary>
        /// P14
        /// </summary>
        Week ,

        /// <summary>
        /// P15
        /// </summary>
        Month,

        /// <summary>
        /// P16
        /// </summary>
        Year,
    }
}
