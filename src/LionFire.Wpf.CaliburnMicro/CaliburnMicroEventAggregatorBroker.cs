using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LionFire.Messaging;

namespace LionFire.Messaging
{
    public class CaliburnMicroEventAggregatorBroker : IMessageBroker
    {
        public static void UseAsDefaultMessageBroker(IEventAggregator eventAggregator = null)
        {
            MBus.DefaultBroker = new CaliburnMicroEventAggregatorBroker()
            {
                agg = eventAggregator ?? new EventAggregator()
            };
        }

        IEventAggregator agg = new EventAggregator();

        public bool HandlerExistsFor(Type messageType)
        {
            return agg.HandlerExistsFor(messageType);
        }

        // REVIEW - expose this marshaller interface?
        public void Publish(object message, MessagingContext context = null)
        {
            Publish(message, context,null);
        }
        public void Publish(object message, MessagingContext context = null, System.Action<System.Action> marshaller = null)
        {
            if (marshaller == null) marshaller = x => x();
            agg.Publish(message, marshaller);
        }

        public void Subscribe(object subscriber, MessagingContext context = null)
        {
            agg.Subscribe(subscriber);
        }

        public void Unsubscribe(object subscriber, MessagingContext context = null)
        {
            agg.Unsubscribe(subscriber);
        }
    }
}
