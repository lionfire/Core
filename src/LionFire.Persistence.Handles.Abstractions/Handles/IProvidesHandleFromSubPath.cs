using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Persistence.Handles
{
    public interface IProvidesHandleFromSubPath
    {
        H<T> GetHandleFromSubPath<T>(params string[] subpathChunks);
        H<T> GetHandleFromSubPath<T>(IEnumerable<string> subpathChunks);
        RH<T> GetReadHandleFromSubPath<T>(params string[] subpathChunks);
        RH<T> GetReadHandleFromSubPath<T>(IEnumerable<string> subpathChunks);
    }
}
