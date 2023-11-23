using LionFire.Inspection.Nodes;
using LionFire.Inspection.ViewModels;
using LionFire.IO;

namespace LionFire.Inspection;

public static class INodeInfoX
{
    public static string GetRWCodes(this INodeInfo? mi) =>
        (mi?.CanRead() == true ? "R" : "")
        + (mi?.CanWrite() == true ? "W" : "");

    public static bool CanRead(this INodeInfo mi) => mi.IODirection.CanRead();
    public static bool CanWrite(this INodeInfo mi) => mi.IODirection.CanWrite();
}
