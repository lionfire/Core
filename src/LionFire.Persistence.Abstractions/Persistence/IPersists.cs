using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence
{
    public interface IPersists
    {
        PersistenceFlags Flags { get; }
    }
}
