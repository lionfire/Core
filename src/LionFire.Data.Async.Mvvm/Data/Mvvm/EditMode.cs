namespace LionFire.Data.Mvvm;

[Flags]
public enum EditMode
{
    Unspecified = 0,
    Create = 1 << 0,
    Delete = 1 << 1,
    Update = 1 << 2,
    Rename = 1 << 3,
    All = Create | Delete | Update | Rename,
}
