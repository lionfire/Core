using System;

namespace LionFire.Persistence.Filesystem
{
    public interface IPersistenceInterceptor
    {

    }
    public interface IFilesystemPersistenceInterceptor : IPersistenceInterceptor
    {
        object Read(string diskPath, Type type = null);

        //bool Write(object obj, string fullDiskPath, Type type, LionSerializer serializer); // TOPORT
    }
}
