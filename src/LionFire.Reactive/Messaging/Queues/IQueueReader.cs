using System;
using System.Collections.Generic;

namespace LionFire.Messaging.Queues
{

    public interface IMessageCollectionReader
    {

    }

    //public interface IHasObservable<T>
    //{
    //    IObservable<T> Observable { get; }
    //}
    public interface IQueueReader 
        //: IHasObservable<MessageEnvelope>
        //: IObservable<MessageEnvelope>
    {
        QueueReaderFlags QueueFlags { get; set; }



        /// <summary>
        /// Return true if handled
        /// </summary>
        event Action<MessageEnvelope> MessageReceived;

        ///// <summary>
        ///// Returns true if there is another message to be handled
        ///// </summary>
        ///// <param name="handler"></param>
        ///// <returns></returns>
        //bool TryHandleNext(Action<MessageEnvelope> handler);



        //int UnhandledMessages { get; set; }
        //void Peek(Func<IQueue, MessageEnvelope, bool /* handled */> handler);

        //event Action<IQueue> ItemsRemoved;
        //event Action<IQueue, string, object> ItemRemoved;

        //IEnumerable<object> Items { get; }

    }

    public interface IUnhandledQueueReader
    {
        IEnumerable<MessageEnvelope> UnhandledMessages { get; }
    }
}
