#nullable enable

namespace LionFire.Resolves;

public interface IStagesSet<T>
{
    T? StagedValue { get; set; }
}
