using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Messaging
{
    public class MessagingContext
    {
        public IMessageBroker Broker { get; set; }

        public List<Predicate<object>> Blacklist { get; set; }
        public List<Predicate<object>> Whitelist { get; set; }

        public bool PassesFilter(object message)
        {
            if (Whitelist != null)
            {
                foreach (var p in Whitelist)
                {
                    if (p(message)) return true;
                }
                return false;
            }
            if (Blacklist != null)
            {
                foreach (var p in Whitelist)
                {
                    if (p(message)) return false;
                }
                return true;
            }
            return true;
        }
    }
}
