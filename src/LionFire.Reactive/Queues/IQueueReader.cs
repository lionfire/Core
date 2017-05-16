using System;
using System.Collections.Generic;

namespace LionFire.Queues
{

    public interface IQueueReader : IObservable<QueueMessageEnvelope>
    {
        QueueFlags Flags { get; set; }

        /// <summary>
        /// Return true if handled
        /// </summary>
        event Action<QueueMessageEnvelope> Received;

        int UnhandledMessages { get; set; }
        void Peek(Func<IQueue, QueueMessageEnvelope, bool /* handled */> handler);

        //event Action<IQueue> ItemsRemoved;
        event Action<IQueue, string, object> ItemRemoved;
        IEnumerable<object> Items { get; }

    }
}
