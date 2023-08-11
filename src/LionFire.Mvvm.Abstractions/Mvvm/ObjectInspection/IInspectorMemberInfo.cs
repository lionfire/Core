using LionFire.IO;
using LionFire.Metadata;

namespace LionFire.Mvvm.ObjectInspection;

public interface IInspectorNodeInfo
{
    string Name { get; }

    /// <summary>
    /// If Order is 0, another default ordering will be used, such as alphabetical.
    /// </summary>
    public int Order { get; set; }
}

public interface IInspectorMemberInfo : IInspectorNodeInfo
{
    Type Type { get; }
    IODirection IODirection => IODirection.Unspecified;

    MemberKind MemberKind { get; }

    IInspectorNode Create(object source);

    RelevanceFlags ReadRelevance { get; set; }

    RelevanceFlags WriteRelevance { get; set; }

    IEnumerable<string> TypeFlags { get; }

}

public static class IInspectorMemberInfoX
{
    public static bool CanRead(this IInspectorMemberInfo mi) => mi.IODirection.IsReadable();
    public static bool CanWrite(this IInspectorMemberInfo mi) => mi.IODirection.IsWritable();
}
