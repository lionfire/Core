#nullable enable

namespace LionFire.Data.Sets;

public interface IStagesSet<T> : ISets
{
    T? StagedValue { get; set; }
    bool HasStagedValue { get; set; }

    void DiscardStagedValue();
}
