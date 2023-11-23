namespace LionFire.Data.Async.Sets;

public interface IAsyncInstantiatesForSet<TValue> : ISetter<TValue>, IStagesSet<TValue>
{
    ITask<TValue> InstantiateValue(bool overwriteStagedValue = false, bool throwOnOverwrite = false);
}
