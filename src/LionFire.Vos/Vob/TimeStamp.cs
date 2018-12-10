using System;

namespace LionFire.Vos
{
    /// <summary>
    /// Wrapper for DateTime
    /// </summary>
    public class TimeStamp
    {
        public DateTime DateTime { get; set; }
        public TimeStamp() { }
        public TimeStamp(DateTime dateTime) { DateTime = dateTime; }
        public static implicit operator TimeStamp(DateTime dateTime) => new TimeStamp(dateTime);
        public static implicit operator DateTime(TimeStamp timeStamp) => timeStamp.DateTime;
    }
}