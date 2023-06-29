
using System;

namespace LionFire.Data;

[Flags]
public enum SetTriggerMode
{
    None = 0,
    OnBlur = 1 << 0,
    OnChange = 1 << 1,
}

[Flags]
public enum ObjectSetTriggerMode // REVIEW use cases and combinations with SetTriggerMode
{
    None = 0,
    ManualPerProperty = 1 << 0, 
    ManualPerObject = 1 << 1,
}