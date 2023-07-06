using MorseCode.ITask;

namespace LionFire.Data.Sets;
public interface IInstantiatesForSet<TValue> : ISets<TValue>, IStagesSet<TValue>
{
    TValue InstantiateValue(bool overwriteStagedValue = false, bool throwOnOverwrite = false);
}