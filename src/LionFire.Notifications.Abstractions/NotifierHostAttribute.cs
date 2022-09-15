
namespace LionFire.Notifications;

[System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class NotifierHostAttribute : Attribute
{
    public NotifierHostAttribute(Type type)
    {
        this.Type = type;
    }
    public Type Type { get; private set; }
}
