using MorseCode.ITask;

namespace LionFire.Data.Async.Sets;
public interface IInstantiatesForSet<TValue> : ISetter<TValue>, IStagesSet<TValue>
{
    TValue InstantiateValue(bool overwriteStagedValue = false, bool throwOnOverwrite = false);
}