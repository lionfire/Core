using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence.Handles
{
    
    public interface IObjectHandleProvider
    {
        IReadHandle<TValue> GetReadHandle<TValue>(TValue value);
        IReadWriteHandle<TValue> GetReadWriteHandle<TValue>(TValue value);
        IWriteHandle<TValue> GetWriteHandle<TValue>(TValue value);
    }
}
