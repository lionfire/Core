using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Messaging.Queues
{
    public abstract class QueueReaderBase
    {
        public virtual QueueReaderFlags QueueFlags { get; set; }

        #region MessageReceived

        public event Action<MessageEnvelope> MessageReceived
        {
            add
            {
                if (messageReceived == null)
                {
                    OnMessageReceivedAttaching();
                }
                messageReceived += value;
            }
            remove
            {
                messageReceived += value;
                if (messageReceived == null)
                {
                    OnMessageReceivedDetached();
                }
            }
        }
        private event Action<MessageEnvelope> messageReceived;

        #endregion

        protected virtual void OnMessageReceivedAttaching() { }
        protected virtual void OnMessageReceivedDetached() { }

        protected virtual void RaiseMessageReceived(MessageEnvelope env)
        {
            foreach (Action<MessageEnvelope> del in messageReceived.GetInvocationList())
            {
                del.Invoke(env);
            }
        }
    }
}

