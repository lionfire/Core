using LionFire.Referencing;

namespace LionFire.Persisters.Expanders;

// TODO
public class ExpanderEvents
{
    // TODO: Hook to specify options on the mount:
    // - unmount timeout
    // - preferred locking mode (r rw)
    // - change tracking
    public event Action<IReference>? Mounting;
}

