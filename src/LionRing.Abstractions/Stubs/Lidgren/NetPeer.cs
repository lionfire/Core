#if NORPC
using System;
using System.Collections.Generic;

namespace Lidgren.Network
{
	public class NetPeer
	{
		public NetPeer()
		{
		}

		public Lidgren.Network.NetConnection Connect(string hostName, int port)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<NetConnection> Connections {
			get;
			set;
		}
	}
}

#endif