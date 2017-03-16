using LionFire.Messaging;
using LionFire.Net.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Net
{
    public class NetEvents
    {
        public static void OnFail(string uri, object request = null)
        {
            new MInternetFailure { Url = uri, Request = request }.Publish();
        }
        public static void OnSuccess(string uri, object request = null)
        {
            new MInternetSuccess { Url = uri, Request = request }.Publish();
        }
    }
}
namespace LionFire.Net.Messages
{
    public class MNetEvent : IMessage
    {
        public string Url { get; set; }
        public object Request { get; set; }
    }
    public class MInternetFailure : MNetEvent
    {


    }
    public class MInternetSuccess : MNetEvent { }
}
