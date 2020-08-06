using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence.Handles
{
    public interface IHasReadWriteHandle { }

    public interface IHasReadWriteHandle<T> : IHasReadWriteHandle
       //where T : class
    {
        IReadWriteHandle<T> ReadWriteHandle { get; }
    }
}
