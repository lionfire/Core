using LionFire.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Settings
{
    public class SettingsOptions
    {
        public bool AutoSave { get; set; }
        public bool SaveOnExit { get; set; }
        public List<IReadWriteHandle> Handles { get; internal set; }
        public List<Type> Types { get; set; } = new List<Type>();
    }
}
