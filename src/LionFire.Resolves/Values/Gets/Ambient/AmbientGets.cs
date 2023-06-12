using LionFire.ExtensionMethods.Poco.Resolvables;
using MorseCode.ITask;

namespace LionFire.Data.Async.Gets
{
    public class AmbientGets<TKey, TValue> : Gets<TKey, TValue>
         where TKey : class
        where TValue : class
    {
        protected override ITask<IGetResult<TValue>> GetImpl() => this.Key.Resolve<TKey, TValue>().AsITask();
    }
}

