using LionFire.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Settings;

public class SettingsOptions
{
    public bool AutoSave { get; set; } 
    public bool SaveOnExit { get; set; }
    public List<IReadWriteHandle> Handles { get;  } = new List<IReadWriteHandle>();
    public List<Type> Types { get; } = new List<Type>();
}
