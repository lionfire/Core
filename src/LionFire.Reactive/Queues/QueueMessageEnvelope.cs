using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Queues
{
    public class QueueMessageEnvelope
    {
        /// <summary>
        /// Identifier for the message queue.  (Examples: this could be a Guid, file name, date stamp, or sequential number.
        /// </summary>
        public string Key { get; set; }
        public IReadHandle<object> Header { get; set; }
        public IReadHandle<object> Payload { get; set; }

        #region Handled

        public bool IsHandled => HandledCount > 0;

        public int HandledCount
        {
            get => handledCount;
        }

        private int handledCount;

        public void OnHandled()
        {
            System.Threading.Interlocked.Increment(ref handledCount);
        }

        #endregion

    }
    
}
