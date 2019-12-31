using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.MultiTyping;
using LionFire.Referencing;
using LionFire.Types;

namespace LionFire.Vos
{

    public interface IVob :
        //IReadOnlyMultiTyped, 
        IReferencable
    {
        string Name { get; }
        IVob Parent { get; }

        #region Cache

        IVob Root { get; }
        string Path { get; }
        IEnumerable<string> PathElements { get; }
        IEnumerable<string> PathElementsReverse { get; }

        #endregion

        T GetNext<T>(bool skipOwn = false) where T : class;
        T GetOwn<T>() where T : class;

        #region Children

        IVob QueryChild(string[] subpathChunks, int index); // Replace with Span?
        //IVob GetChild(IEnumerable<string> subpathChunks); // Does this exist? If so, Replace with Span?
        IVob GetChild(IEnumerator<string> subpathChunks);

        #endregion

        IVob this[int index, string[] subpathChunks] { get; }
        IVob this[string subpath] { get; }

    }
}
