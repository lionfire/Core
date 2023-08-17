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