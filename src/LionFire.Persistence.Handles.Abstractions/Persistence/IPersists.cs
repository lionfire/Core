using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence;

public interface IPersists<T> : IPersists
     //where T : class
{
    IPersistenceSnapshot<T> PersistenceState { get; }
}

public interface IPersists
{
    
    PersistenceFlags Flags { get; }
}
public interface IPersistsInternal : IPersists
{
    new PersistenceFlags Flags { get;  set; }
}