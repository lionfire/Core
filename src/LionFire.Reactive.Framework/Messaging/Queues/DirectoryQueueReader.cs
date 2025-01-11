using System;
using System.Linq;
using LionFire.Instantiating;

namespace LionFire.Messaging.Queues
{
    public class TDirectoryQueueReader
    {
        /// <summary>
        /// Polling disabled if not a positive integer
        /// </summary>
        public int PollMillisecondsInterval { get; set; }

        /// <summary>
        /// Be instantly notified about incoming messages
        /// </summary>
        public bool WatchForIncomingMessages { get; set; }

        public string QueueDataUri { get; set; }
    }

    public abstract class DirectoryQueueReader<TTemplate> : QueueReaderBase
        where TTemplate : TDirectoryQueueReader, new()
    {
        protected Guid Guid = Guid.NewGuid();
        
        public static bool AutoCleanDeadQueues { get; set; } = true;

        #region Subdirectories
        

        #endregion

        public TTemplate Template { get; set; } = new TTemplate
        {
            PollMillisecondsInterval = 1000,
        };
    }    
}
