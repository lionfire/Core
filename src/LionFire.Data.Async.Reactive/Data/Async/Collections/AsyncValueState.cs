namespace LionFire.Data.Collections;

public enum AsyncValueState
{
    Unspecified = 0,
    Loaded = 1 << 0,
    Listening = 1 << 1,
    Invalid = 1 << 2,
    Deleted = 1 << 3,
}
