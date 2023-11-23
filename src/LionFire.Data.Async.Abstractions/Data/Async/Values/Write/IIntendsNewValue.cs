namespace LionFire.Data.Async.Sets;

public interface IIntendsNewValue
{
    /// <summary>
    /// Sets HasStagedValue to true, even if StagedValue is null.
    /// 
    /// Invoke this on new IStatelessAsyncValue instances when you are intending to create a new Value but don't know what that value will be yet (otherwise, you could set IWriteWrapper.Value), in order to avoid a get_Value from attempting 
    /// to lazily resolve a Value that you don't expect to exist.
    /// </summary>
    void IntendNewValue();
}
