using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Referencing;
using LionFire.Persistence.Handles;
using LionFire.Persistence;
using LionFire.ObjectBus.Handles;

namespace LionFire.ObjectBus.Filesystem
{
    public class FsOBus : SingletonOBusBase<FsOBus, FsOBase, FileReference>
    {
        //public H<T> GetHandle<T>(FileReference reference, T handleObject = default) => new OBaseHandle<T>(reference, DefaultOBase, handleObject);
        //public RH<T> GetReadHandle<T>(FileReference reference, T handleObject = default) => new OBaseReadHandle<T>(reference, DefaultOBase, handleObject);
        
        #region FUTURE // Maybe

        //public static FilesystemOBus Instance => ManualSingleton<FilesystemOBus>.GuaranteedInstance;

        //public IOBase GetOBaseForConnectionString(string connectionString)
        //{
        //    if (connectionString != null)
        //    {
        //        throw new ArgumentException("connectionString must be null");
        //    }

        //    // For now, there is only one instance, representing the local file system.
        //    return FsOBase.Instance;
        //}

        #endregion

    }
}

