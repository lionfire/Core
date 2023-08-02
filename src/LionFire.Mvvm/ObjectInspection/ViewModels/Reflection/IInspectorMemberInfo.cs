using LionFire.IO;

namespace LionFire.Mvvm.ObjectInspection;

public interface IInspectorMemberInfo
{
    string Name { get; }
    Type Type { get; }

    /// <summary>
    /// If Order is 0, another default ordering will be used, such as alphabetical.
    /// </summary>
    public int Order { get; set; }

    IODirection IODirection => IODirection.Unspecified;

    MemberKind MemberKind { get; }

    IMemberVM Create(object obj);
}

public static class IInspectorMemberInfoX
{
    public static bool CanRead(this IInspectorMemberInfo mi) => mi.IODirection.IsReadable();
    public static bool CanWrite(this IInspectorMemberInfo mi) => mi.IODirection.IsWritable();
}
