#if NORPC
using System;

namespace Lidgren.Network
{
	public class NetOutgoingMessage
	{
	}
	public class NetIncomingMessage{
		public int LengthBytes {
			get;
			set;
		}

		public int PositionInBytes {
			get;
			set;
		}
}
	public class NetPeerConfiguration{}
}

#endif