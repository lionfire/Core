using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence.Handles
{
    public interface ICollectionHandleProvider : IReadCollectionHandleProvider
    {
        C<T> GetCollectionHandle<T>(IReference reference);
        //C<T> GetCollectionHandle<T>(IReference reference/*, T handleObject = default(T)*/);

    }

    public interface ICollectionHandleProvider<TReference>
    where TReference : IReference
    {
        C<T> GetCollectionHandle<T>(TReference reference);
    }
}
