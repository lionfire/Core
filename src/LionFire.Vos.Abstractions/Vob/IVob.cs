using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.FlexObjects;
using LionFire.MultiTyping;
using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Types;

namespace LionFire.Vos
{
    public interface IVob :
        //IReadOnlyMultiTyped, 
        IReferencable,
        IFlex
    {
        new IVosReference Reference { get; }
        string Name { get; }
        IVob Parent { get; }

        #region Cache

        IRootVob Root { get; }
        string Path { get; }
        IEnumerable<string> PathElements { get; }
        IEnumerable<string> PathElementsReverse { get; }

        #endregion

        T AcquireNext<T>(int minDepth = 0, int maxDepth = -1) where T : class;
        T AcquireOwn<T>() where T : class; 
        IEnumerable<T> Acquire<T>(int minDepth = 0, int maxDepth = -1) where T : class;

        #region Children

        IVob QueryChild(string subpath); 
        IVob QueryChild(string[] subpathChunks, int index = 0); // Replace with Span?
        IVob QueryChild(IVosReference reference); 
        //IVob GetChild(IEnumerable<string> subpathChunks); // Use IVobExtensions Extension method instead.  
        IVob GetChild(string subpath); 
        IVob GetChild(IEnumerator<string> subpathChunks);
        IVob GetChild(string[] subpathChunks, int index = 0);

        #endregion

        IVob this[string[] subpathChunks, int index = 0] { get; }
        IVob this[string subpath] { get; }
        IVob this[IVosReference reference] { get; }

        IReadHandle<T> GetReadHandle<T>(T preresolvedValue = default);
        IReadWriteHandle<T> GetReadWriteHandle<T>(T preresolvedValue = default);
        IWriteHandle<T> GetWriteHandle<T>(T prestagedValue = default);
    }

}
