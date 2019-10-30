using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence
{
    public interface IPersisted
    {
        PersistenceState State { get; }
    }
}
