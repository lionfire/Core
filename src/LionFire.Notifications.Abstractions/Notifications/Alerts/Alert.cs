using System;

namespace LionFire.Notifications
{
    public class Alert
    {
        public string User { get; set; }
        public string Profile { get; set; }

        public string Category { get; set; }
        public string Object { get; set; }

        public string Title { get; set; }
        public string Message { get; set; }
        public string Verbose { get; set; }
        //public string EncodedMessage { get; set; }
        public int Priority { get; set; }
        public DateTime Date { get; set; }
        public int DispatchFailCount { get; set; }
    }
}
