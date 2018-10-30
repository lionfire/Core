using System.Collections.Generic;

namespace LionFire.Synchronization
{
    public interface ISyncConnectionInfo
    {
        IEnumerable<ISyncConnectionMode> AttachModes { get; }
        ISyncConnectionMode CurrentModes { get; }
    }

}
