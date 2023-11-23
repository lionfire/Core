using LionFire.Referencing;

namespace LionFire.Serialization;

public interface INotifyDeserialized
{
    //void OnSerializing();
    //void OnSerialized();
    void OnDeserialized();
}

// TODO: Replace parameters with PersistenceOperation, which includes Reference
public interface INotifyReferenceDeserialized<TReference>
    where TReference : IReference
{
    //void OnSerializing();
    //void OnSerialized();
    void OnDeserialized(TReference reference);
}
