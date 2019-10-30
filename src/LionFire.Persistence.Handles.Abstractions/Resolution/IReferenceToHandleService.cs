using LionFire.Referencing;
using System;
using System.Collections.Generic;

namespace LionFire.Persistence.Handles
{
    public interface IReferenceToHandleService //: IReferenceToHandleProvider<IReference>
    {
        IReadWriteHandleProvider GetReadWriteHandleProvider(IReference input);
        IReadHandleProvider GetReadHandleProvider(IReference input);
        IWriteHandleProvider GetWriteHandleProvider(IReference input);
        

        ICollectionHandleProvider GetCollectionHandleProvider(IReference input);

        //IReadWriteHandleProvider GetHandleProvider(Type referenceType);
        //IReadHandleProvider GetReadHandleProvider(Type referenceType);
    }

    //public interface IReferenceToHandleProviderProvider<TReference>
    //    where TReference : IReference
    //{
    //    IEnumerable<IReadWriteHandleProvider> GetHandleProviders(IReference input);
    //    IEnumerable<IReadHandleProvider> GetReadHandleProviders(IReference input);
    //}


}
