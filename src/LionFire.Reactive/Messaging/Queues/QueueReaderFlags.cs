using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Messaging.Queues
{
    [Flags]
    public enum QueueReaderFlags
    {
        None,

        Default = None,

        DiscardUnhandledMessages,

        /// <summary>
        /// Require each message be handled in order.
        /// </summary>
        Sequential,

        /// <summary>
        /// If not set, handling will stop after one handler has handled the message
        /// </summary>
        MultipleHandlers,

        /// <summary>
        /// If not set, messages will be deleted if one handler successfully handles it
        /// </summary>
        ExplicitDelete,
    }

}
