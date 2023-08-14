using LionFire.IO;
using LionFire.Metadata;

namespace LionFire.Inspection;

public enum InspectorNodeKind
{
    Unspecified = 0,

    /// <summary>
    /// 
    /// </summary>
    Object = 1 << 0,
    Group = 1 << 1,
    Members = 1 << 2,
    Enumerable = 1 << 3,

}
public class InspectorInfoContext
{
    public IServiceProvider ServiceProvider { get; }

    public InspectorInfoContext(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }
}

public interface IInspectorNodeInfo
{
    string Name { get; }

    InspectorNodeKind NodeKind { get; }

    /// <summary>
    /// If Order is 0, another default ordering will be used, such as alphabetical.
    /// </summary>
    public float Order { get; set; }

    InspectorInfoContext Context { get; }

    IInspectorNode CreateNode(params object?[]? parameters);
}

public interface IInspectorGroupInfo
{
    bool Flatten { get; }

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
