#if DISABLED
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Persistence.Handles
{
    public interface ISubpathHandleProvider
    {
        IReadWriteHandleBase<T> GetReadWriteHandleFromSubPath<T>(params string[] subpathChunks);
        IReadWriteHandleBase<T> GetReadWriteHandleFromSubPath<T>(IEnumerable<string> subpathChunks);
        IReadHandleBase<T> GetReadHandleFromSubPath<T>(params string[] subpathChunks);
        IReadHandleBase<T> GetReadHandleFromSubPath<T>(IEnumerable<string> subpathChunks);
        //IWriteHandleBase<T> GetWriteHandleFromSubPath<T>(IEnumerable<string> chunks);
    }
}
#endif