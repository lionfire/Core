using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Persistence.Handles
{
    public interface IProvidesHandleFromSubPath
    {
        IReadWriteHandleBase<T> GetHandleFromSubPath<T>(params string[] subpathChunks);
        IReadWriteHandleBase<T> GetHandleFromSubPath<T>(IEnumerable<string> subpathChunks);
        IReadHandleBase<T> GetReadHandleFromSubPath<T>(params string[] subpathChunks);
        IReadHandleBase<T> GetReadHandleFromSubPath<T>(IEnumerable<string> subpathChunks);
    }
}
