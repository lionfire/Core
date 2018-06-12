using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Messaging
{
    public interface IMessageBroker
    {
        bool HandlerExistsFor(Type messageType);
        void Publish(object message, MessagingContext context = null);
        void Subscribe(object subscriber, MessagingContext context = null);
        void Unsubscribe(object subscriber, MessagingContext context = null);
    }
}
