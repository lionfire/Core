#define ConcurrentHandles
#define WARN_VOB
//#define INFO_VOB
#define TRACE_VOB

using System;

namespace LionFire.Vos
{
    public class EventBase
    {
        public readonly DateTime EventTime = DateTime.Now;

        public bool IsBefore { get; set; }
    }

}
