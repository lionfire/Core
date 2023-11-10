#if NORPC
using System;

namespace Lidgren.Network
{
	public enum NetConnectionStatus{}

	public class NetConnection
	{
		public NetConnectionStatus Status {
			get;
			set;
		}

//		public NetPeer NetPeer{get;set;}
		public NetPeer Peer{get;set;}

		public object Tag {
			get;
			set;
		}

		public Uri RemoteEndpoint {
			get;
			set;
		}

		public NetConnection()
		{
		}
	}
}

#endif