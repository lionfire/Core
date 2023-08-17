using LionFire.IO;
using LionFire.Metadata;

namespace LionFire.Inspection;

public interface IInspectorMemberInfo : INodeInfo
{
    Type Type { get; }
    IODirection IODirection => IODirection.Unspecified;

    MemberKind MemberKind { get; }

    //IInspectorMember Create(object source);

    RelevanceFlags ReadRelevance { get; set; }

    RelevanceFlags WriteRelevance { get; set; }

    IEnumerable<string> TypeFlags { get; }

}

public static class IInspectorMemberInfoX
{
    public static bool CanRead(this IInspectorMemberInfo mi) => mi.IODirection.IsReadable();
    public static bool CanWrite(this IInspectorMemberInfo mi) => mi.IODirection.IsWritable();
}
