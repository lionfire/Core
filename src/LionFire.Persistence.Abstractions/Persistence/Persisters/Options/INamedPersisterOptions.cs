#nullable enable


namespace LionFire.Persistence.Persisters;

public interface INamedPersisterOptions
{
    string PersisterName { get; set; }
}
