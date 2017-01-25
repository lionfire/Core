using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Messaging
{    
    

    public class MessageBroker : IMessageBroker
    {
        public bool HandlerExistsFor(Type messageType)
        {
            return false;
        }

        public void Publish(object message, MessagingContext context = null)
        {
        }

        public void Subscribe(object subscriber, MessagingContext context = null)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(object subscriber, MessagingContext context = null)
        {
            throw new NotImplementedException();
        }
        
    }
}
