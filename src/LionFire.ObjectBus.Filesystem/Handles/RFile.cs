using System;
using System.Threading.Tasks;
using LionFire.Referencing;

namespace LionFire.ObjectBus.Handles
{
    public class RFile<T> : RBase<T>
        where T : class
    {
        public override Task<bool> TryRetrieveObject() => throw new NotImplementedException();
    }
}
