using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.LionRing
{
    public interface IConnectable
    {
        ConnectionState ConnectionState { get; }
        ConnectionState DesiredConnectionState { get; set; }        
        event Action<IConnectable> ConnectionStateChanged;

    }

    public static class IConnectableExtensions
    {
        public static bool IsConnected(this IConnectable connectable)
        {
            return connectable.ConnectionState == ConnectionState.Connected;
        }
        public static bool IsConnecting(this IConnectable connectable)
        {
            return connectable.ConnectionState == ConnectionState.Connecting;
        }
    }
}
