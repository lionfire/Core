using LionFire.Referencing;
using System;
using System.Collections.Generic;

namespace LionFire.Persistence.Handles
{
    public interface IReferenceToHandleService //: IReferenceToHandleProvider<IReference>
    {
        #region Read

        #region Provider

        IReadHandleProvider GetReadHandleProvider(IReference input);
        IReadHandleProvider<TReference> GetReadHandleProvider<TReference>(TReference input)
            where TReference : IReference
            ;

        #endregion

        #region Creator

        IReadHandleCreator GetReadHandleCreator(IReference input);
        IReadHandleCreator<TReference> GetReadHandleCreator<TReference>(TReference input)
            where TReference : IReference
            ;

        #endregion

        #endregion

        #region ReadWrite

        #region Provider

        IReadWriteHandleProvider GetReadWriteHandleProvider(IReference input);
        IReadWriteHandleProvider<TReference> GetReadWriteHandleProvider<TReference>(TReference input)
            where TReference : IReference
            ;

        #endregion

        #region Creator

        IReadWriteHandleCreator GetReadWriteHandleCreator(IReference input);
        IReadWriteHandleCreator<TReference> GetReadWriteHandleCreator<TReference>(TReference input)
            where TReference : IReference
            ;

        #endregion

        #endregion

        #region Write

        #region Provider

        IWriteHandleProvider GetWriteHandleProvider(IReference input);
        
        IWriteHandleProvider<TReference> GetWriteHandleProvider<TReference>(TReference input)
            where TReference : IReference
            ;

        #endregion

        #region Creator

        IWriteHandleCreator GetWriteHandleCreator(IReference input);

        IWriteHandleCreator<TReference> GetWriteHandleCreator<TReference>(TReference input)
            where TReference : IReference
            ;

        #endregion

        #endregion

        #region Collection

        ICollectionHandleProvider GetCollectionHandleProvider(IReference input);

        #endregion

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
