using System;

namespace Lidgren.Network;

#if NORPC
public class NetException : Exception
{

	public NetException(string msg) : base(msg)
	{
	}
}

#endif