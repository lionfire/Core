﻿using LionFire.Referencing;
using System;
using System.Collections.Generic;

// TODO: enable nullable, make Get methods non-nullable, and add TryGet methods that are nullable in return type.

namespace LionFire.Persistence.Handles;

public interface IReferenceToHandleService //: IReferenceToHandleService<IReference>
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

// TODO: OPTIMIZE - optimization of non-generic IReferenceToHandleService - worth it?
// If doing this, make IReferenceToHandleService inherit from  IReferenceToHandleService<IReference>
//public interface IReferenceToHandleService<TReference>
//    where TReference : IReference
//{
//}

//public interface IReferenceToHandleProviderProvider<TReference>
//    where TReference : IReference
//{
//    IEnumerable<IReadWriteHandleProvider> GetHandleProviders(IReference input);
//    IEnumerable<IReadHandleProvider> GetReadHandleProviders(IReference input);
//}
