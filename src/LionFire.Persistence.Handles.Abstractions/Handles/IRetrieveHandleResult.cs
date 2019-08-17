using LionFire.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Referencing
{
    public interface IRetrieveHandleResult<out T> : IReadResult
    //, IRetrieveResult<out T> // FUTURE: Implement Object => Handle.Object?
    {
        //T Object => ReadHandle.Object; // FUTURE: Implement Object => Handle.Object?

        RH<T> ReadHandle { get; }
    }
}
