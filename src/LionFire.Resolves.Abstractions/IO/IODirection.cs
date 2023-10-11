using System.Reflection;

namespace LionFire.IO;

[Flags]
public enum IODirection
{
    Unspecified = 0,
    Write = 1 << 0,
    Read = 1 << 1,
    ReadWrite = Read | Write,
}

public interface IIOCapable
{
    IODirection IODirection { get; }

    bool CanRead => IODirection.CanRead();
    bool CanWrite => IODirection.CanWrite();
}


public static class IODirectionX
{
    public static bool CanRead(this IODirection d) => d.HasFlag(IODirection.Read);
    public static bool CanWrite(this IODirection d) => d.HasFlag(IODirection.Write);
    public static IODirection GetIODirection(this PropertyInfo propertyInfo) => (propertyInfo.CanRead ? IODirection.Read : IODirection.Unspecified) | (propertyInfo.CanWrite ? IODirection.Write : IODirection.Unspecified);

    public static bool HasValue(Type type) => type != typeof(void) && type != typeof(Task) && type != typeof(ValueTask) && type != typeof(ITask);

    public static IODirection GetIODirection(this MethodInfo methodInfo) => (HasValue(methodInfo.ReturnType) ? IODirection.Read : IODirection.Unspecified) | (methodInfo.GetParameters().Length > 0 ? IODirection.Write : IODirection.Unspecified);
}