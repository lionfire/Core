namespace LionFire.Inspection;

[Flags]
public enum InspectorVisibility
{
    Unspecified = 0,
    
    Detail = 1 << 1,
    ExtraDetail = 1 << 2,

    Hidden = 1 << 3,

    Secret = 1 << 5,

    Moderator = 1 << 7,
    Admin = 1 << 8,
    SuperAdmin = 1 << 9,

    TechAdmin = 1 << 14,

    Dev = 1 << 15,
}