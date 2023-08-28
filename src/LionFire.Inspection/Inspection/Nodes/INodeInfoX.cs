using LionFire.Inspection.Nodes;
using LionFire.IO;

namespace LionFire.Inspection;

public static class INodeInfoX
{
    public static bool CanRead(this INodeInfo mi) => mi.IODirection.IsReadable();
    public static bool CanWrite(this INodeInfo mi) => mi.IODirection.IsWritable();
}
