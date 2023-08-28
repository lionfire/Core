namespace LionFire.Inspection;

[Flags]
public enum InspectedObjectType
{
    Unspecified,
    Normal = 1 << 0,
    Hidden = 1 << 1,
    Internal = 1 << 2,
}
