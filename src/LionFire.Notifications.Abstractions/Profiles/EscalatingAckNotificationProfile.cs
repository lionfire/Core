using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Profiles
{
    //public class Job : IJob
    //{

    //}

    //public class JobState
    //{

    //}

    //public interface IJob
    //{

    //}

    public class HttpJob
    {

    }

    public class EmailJob
    {

    }

    public class EmailJobParameters
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool? Priority { get; set; }
    }

    public class EscalatingAckNotificationProfileLevel
    {
        public string Action { get; set; }
    }

    public class EscalatingAckNotificationProfile
    {
        public List<EscalatingAckNotificationProfileLevel> Levels { get; set; }
    }
}
