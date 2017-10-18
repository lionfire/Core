using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{

    public interface ITreeHandle
    {
        IHandle this[string subpath] { get; }
        IHandle this[IEnumerator<string> subpathChunks] { get; }
        IHandle this[IEnumerable<string> subpathChunks] { get; }
        IHandle this[int index, string[] subpathChunks] { get; }
        IHandle this[params string[] subpathChunks] { get; }

#if !AOT // -- revIEW - do i need this? 150309
        IHandle<T> GetHandle<T>(params string[] subpathChunks) where T : class;
        IHandle<T> GetHandle<T>(IEnumerable<string> subpathChunks) where T : class;
#endif
    }
}
