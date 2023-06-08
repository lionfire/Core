#nullable enable

namespace LionFire.Data.Async.Gets;

public interface IStagesSet<T>
{
    T? StagedValue { get; set; }
}
