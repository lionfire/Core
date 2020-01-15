using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.MultiTyping;
using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Types;

namespace LionFire.Vos
{

    public interface IVob :
        //IReadOnlyMultiTyped, 
        IReferencable
    {
        new IVosReference Reference { get; }
        string Name { get; }
        IVob Parent { get; }

        #region Cache

        IVob Root { get; }
        string Path { get; }
        IEnumerable<string> PathElements { get; }
        IEnumerable<string> PathElementsReverse { get; }
        object Value { get; set; }

        #endregion

        T GetNext<T>(bool skipOwn = false) where T : class;
        T GetOwn<T>() where T : class;

        #region Children

        IVob QueryChild(string[] subpathChunks, int index); // Replace with Span?
        //IVob GetChild(IEnumerable<string> subpathChunks); // Use IVobExtensions Extension method instead.  
        IVob GetChild(IEnumerator<string> subpathChunks);

        #endregion

        IVob this[string[] subpathChunks, int index = 0] { get; }
        IVob this[string subpath] { get; }


        IReadHandle<T> GetReadHandle<T>();
        IReadWriteHandle<T> GetReadWriteHandle<T>();
        IWriteHandle<T> GetWriteHandle<T>();
    }

}
