using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Messaging
{

    public static class MBus
    {
        public static IMessageBroker DefaultBroker { get { return ManualSingleton<IMessageBroker>.GetGuaranteedInstance<MessageBroker>(); } set { ManualSingleton<IMessageBroker>.Instance = value; } }

        /// <summary>
        /// False sets DefaultBroker to NoopMessageBroker, True sets to MessageBroker.  Set DefaultBroker yourself to set your own type.
        /// </summary>
        public static bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (isEnabled == value) return;
                isEnabled = value;
                if (isEnabled)
                {
                    ManualSingleton<IMessageBroker>.Instance = new MessageBroker();
                }
                else
                {
                    ManualSingleton<IMessageBroker>.Instance = new NoopMessageBroker();
                }
            }
        }
        private static bool isEnabled = true;


        public static void Publish(object obj, MessagingContext context = null)
        {
            var broker = context?.Broker ?? DefaultBroker;
            broker.Publish(obj, context);
        }

        public static void Subscribe<T>(object subscriber, MessagingContext context = null)
        {
            var broker = context?.Broker ?? DefaultBroker;
            broker.Subscribe(subscriber, context);
        }

        #region IMessage convenience

        public static void Publish(this IMessage obj, MessagingContext context = null)
        {
            Publish((object)obj, context);
        }

        #endregion
    }


}
