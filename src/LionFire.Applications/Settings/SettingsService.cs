using LionFire.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Applications.Settings
{
    public interface ISettingsService
    {
        IEnumerable<IReadWriteHandle> SettingsObjects { get; }
    }

    public class SettingsService : ISettingsService
    {
        public static SettingsService Instance { get; set; }

        IEnumerable<IReadWriteHandle> ISettingsService.SettingsObjects => SettingsObjects;
        public List<IReadWriteHandle> SettingsObjects { get; } = new List<IReadWriteHandle>();
    }
}
