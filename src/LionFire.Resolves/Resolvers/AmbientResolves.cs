using LionFire.ExtensionMethods.Poco.Resolvables;
using MorseCode.ITask;

namespace LionFire.Resolves
{
    public class AmbientResolves<TKey, TValue> : Resolves<TKey, TValue>
    {
        public override ITask<IResolveResult<TValue>> ResolveImpl() => this.Key.Resolve<TKey, TValue>().AsITask();
    }
}

