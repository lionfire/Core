
using MorseCode.ITask;

namespace LionFire.Data.Async.Gets
{
    // TODO?
    //public interface IWriteableLazilyResolves
    //{
    //    /// <summary>
    //    /// Invoke this on new IGets instances when you are intending to create a new Value but don't know what that value will be yet (otherwise, you could set IWriteWrapper.Value), in order to avoid a get_Value from attempting 
    //    /// to lazily resolve a Value that you don't expect to exist.
    //    /// </summary>
    //    void IntendNewValue();
    //}

    public interface IWritableLazilyResolves<out TValue> : ILazilyResolves<TValue>
    {
        ITask<IGetResult<TValue>> GetOrInstantiateValue();
    }
}
