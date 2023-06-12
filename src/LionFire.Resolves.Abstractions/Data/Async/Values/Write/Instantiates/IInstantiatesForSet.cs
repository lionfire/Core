using MorseCode.ITask;

namespace LionFire.Data.Async.Sets;
public interface IInstantiatesForSet<TValue> : ISets<TValue>, IStagesSet<TValue>
{
    TValue InstantiateValue(bool overwriteStagedValue = false, bool throwOnOverwrite = false);
}