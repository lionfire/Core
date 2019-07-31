using LionFire.Referencing;
using System;
using System.Collections.Generic;

namespace LionFire.Referencing.Handles
{
    public interface IReferenceToHandleService //: IReferenceToHandleProvider<IReference>
    {
        IHandleProvider GetHandleProvider(IReference input);
        IReadHandleProvider GetReadHandleProvider(IReference input);

        ICollectionHandleProvider GetCollectionHandleProvider(IReference input);

        //IHandleProvider GetHandleProvider(Type referenceType);
        //IReadHandleProvider GetReadHandleProvider(Type referenceType);
    }

    //public interface IReferenceToHandleProviderProvider<TReference>
    //    where TReference : IReference
    //{
    //    IEnumerable<IHandleProvider> GetHandleProviders(IReference input);
    //    IEnumerable<IReadHandleProvider> GetReadHandleProviders(IReference input);
    //}


}
