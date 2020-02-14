using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace LionFire.Events
{
    public interface IEventInvoker
    {
        bool HasSubscribers { get; }
        object OnEvent(params object[] args);

        void AddHandler(Delegate handler);
        void RemoveHandler(Delegate handler);
    }

    public class EventInvoker
        : IEventInvoker
    //<InterfaceType, DelegateType> : IEventInvoker
    //where InterfaceType : class
    //where DelegateType : class
    {
        protected Delegate Subscribers;

        //public event Func<object, object[], object> Fired
        //{
        //    add
        //    {
        //        if (Subscribers == null)
        //        {
        //            Subscribers = value as Delegate;
        //        }
        //        else
        //        {
        //            Subscribers = Delegate.Combine(Subscribers, value);
        //        }
        //    }
        //    remove
        //    {
        //        if (Subscribers == null)
        //        {
        //            Subscribers = value as Delegate;
        //        }
        //        else
        //        {
        //            Subscribers = Delegate.Remove(Subscribers, value);
        //        }
        //    }
        //}

        //public void Subscribe(IObserver<object[]> observer)
        //{
        //}

        public void AddHandler(Delegate d)
        {
            Subscribers = Delegate.Combine(Subscribers, d);
            //Subscribers =  (Subscribers as Delegate) + (d => () Delegate);
        }

        public void RemoveHandler(Delegate d)
        {
            Subscribers = Delegate.Remove(Subscribers, d);
            //Subscribers =  (Subscribers as Delegate) + (d => () Delegate);
        }

        public bool HasSubscribers { get { return Subscribers != null; } }

        //internal LionRpcInterceptor<InterfaceType> Interceptor;
        internal string eventName;
        internal Type delegateType;

        public EventInvoker(
            //LionRpcInterceptor<InterfaceType> interceptor, 
            string eventName,
            Type delegateType
            )
        {
            //this.Interceptor = interceptor;
            this.eventName = eventName;
            this.delegateType = delegateType;
        }

        static EventInvoker()
        {
            OnEventMethodInfo = typeof(EventInvoker).GetMethod("OnEvent");
        }
        internal static MethodInfo OnEventMethodInfo;

        public object OnEvent(params object[] args)
        {
            //l.Trace("IEventInvoker.OnEvent: Raising event: " + eventName);
            var registeredDelegate = (MulticastDelegate)this.Subscribers;

            object result = null;
            if (registeredDelegate == null)
            {
                l.Trace("No delegate registered for event: " + eventName + " (TODO: Unsubscribe from remote event?)");
                return null;
            }

            var invocationList = registeredDelegate.GetInvocationList();
            foreach (var delegateMember in invocationList)
            {
                try
                {
                    result = delegateMember.DynamicInvoke(args);
                }
                catch (Exception ex)
                {
                    l.Error("Local event handler threw exception for event: " + eventName + " Exception: " + ex.ToString());
                }
            }

            return result;
        }
        private static readonly ILogger l = Log.Get();

    }
}
