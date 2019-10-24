using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence.Handles
{
    public interface ICollectionHandleProvider : IReadCollectionHandleProvider
    {
        HC<T> GetCollectionHandle<T>(IReference reference);

    }

    public interface ICollectionHandleProvider<TReference>
    where TReference : IReference
    {
        //HC<TValue> GetCollectionHandle<TValue>(TReference reference);
    }
}
