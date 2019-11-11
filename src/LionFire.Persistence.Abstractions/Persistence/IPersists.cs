using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence
{
    public interface IPersists<T> : IPersists
         //where T : class
    {
        PersistenceSnapshot<T> PersistenceState { get; }
    }

    public interface IPersists
    {
        
        PersistenceFlags Flags { get; }
    }
}
