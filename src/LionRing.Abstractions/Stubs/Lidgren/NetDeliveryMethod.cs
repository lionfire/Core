#if NoLidgren || NORPC
using System;

namespace Lidgren.Network;

public enum NetDeliveryMethod
{
	Unknown = 0,
	Unreliable = 1,
	UnreliableSequenced = 2,
	ReliableUnordered = 34,
	ReliableSequenced = 35,
	ReliableOrdered = 67,
}

#endif