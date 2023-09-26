namespace LionFire.Inspection;

[Flags]
public enum InspectorNodeKind
{
    Unspecified = 0,

    /// <summary>
    /// 
    /// </summary>
    Object = 1 << 0,
    Group = 1 << 1,
    Summary = 1 << 2,

    Enumerable = 1 << 3,


    Data = 1 << 4,
    Event = 1 << 5,
    Method = 1 << 6,

    Transform = 1 << 8

    // Maybe:
    //Async = 1 << 16,

    //Member = Data | Event | Method,
}