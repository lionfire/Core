#nullable enable

namespace LionFire.Data.Sets;

public interface IStagesSet<T> : IReadStagesSet<T>, IWriteStagesSet<T>
{
}

public interface IReadStagesSet<out T> : ISets
{
    T? StagedValue { get; }    
}

public interface IWriteStagesSet<in T> : ISets
{
    T? StagedValue { set; }
    bool HasStagedValue { get; set; }

    void DiscardStagedValue();
}


public interface IStagesSetWithPersistenceFlags<T> : IStagesSet<T>
{
    PersistenceFlags Flags { get; set; }
}