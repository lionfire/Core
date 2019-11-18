//#define TRACE_SAVE
#define TRACE_LOAD

using System;
//using LionFire.Extensions.Collections;

namespace LionFire.IO.Filesystem // MOVE to LionFire.IO.Filesystem
{
    public interface IFsPersistenceInterceptor
    {
        object Read(string diskPath, Type type = null);

        //bool Write(object obj, string fullDiskPath, Type type, LionSerializer serializer); // TOPORT
    }
}
