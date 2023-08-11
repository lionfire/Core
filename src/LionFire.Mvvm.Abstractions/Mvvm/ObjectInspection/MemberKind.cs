namespace LionFire.Mvvm.ObjectInspection;

public enum MemberKind
{
    Unspecified,
    Data = 1 << 0,
    Method = 1 << 1,
    Event = 1 << 2,
}
