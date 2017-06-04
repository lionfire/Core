using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Messaging
{
    /// <summary>
    /// Default  IMessageBroker implementation
    /// </summary>
    public class NoopMessageBroker : IMessageBroker
    {
        public bool HandlerExistsFor(Type messageType)
        {
            return false;
        }

        public void Publish(object message, MessagingContext context)
        {
        }

        public void Subscribe(object subscriber, MessagingContext context)
        {            
        }
        public void Unsubscribe(object subscriber, MessagingContext context)
        {
        }
    }
}
