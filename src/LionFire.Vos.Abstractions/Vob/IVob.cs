using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LionFire.FlexObjects;
using LionFire.MultiTyping;
using LionFire.Ontology;
using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Structures;
using LionFire.Types;

namespace LionFire.Vos
{
    public interface IVob :
        //IReadOnlyMultiTyped, 
        IReferencable,
        IReferencable<IVobReference>,
        IEnumerable<IVob>, // Does not trigger a Retrieve (IO) operation -- it is just the child Vobs currently tracked
        IFlex,
        IAcquires,
        IParented<IVob>

    {
        string Name { get; }
        //IVobReference Reference { get; }
        //IVob Parent { get; }

        #region Cache

        IRootVob Root { get; }
        string Path { get; }
        IEnumerable<string> PathElements { get; }
        IEnumerable<string> PathElementsReverse { get; }

        #endregion

        #region Nodes

        T Acquire<T>(int minDepth = 0, int maxDepth = -1) where T : class;
        T AcquireOwn<T>() where T : class; 
        IEnumerable<T> AcquireEnumerator<T>(int minDepth = 0, int maxDepth = -1) where T : class;

        #endregion

        #region Children

        IEnumerable<KeyValuePair<string, IVob>> Children { get; }
        IVob QueryChild(string subpath); 
        IVob QueryChild(string[] subpathChunks, int index = 0); // Replace with Span?
        IVob QueryChild(IVobReference reference); 
        //IVob GetChild(IEnumerable<string> subpathChunks); // Use IVobExtensions Extension method instead.  
        IVob GetChild(string subpath); 
        IVob GetChild(IEnumerator<string> subpathChunks);
        IVob GetChild(string[] subpathChunks, int index = 0);


        IVob this[string[] subpathChunks, int index = 0] { get; }
        IVob this[string subpath] { get; }
        IVob this[IVobReference reference] { get; }

        #endregion

        #region Handles

        IReadHandle<T> GetReadHandle<T>(T preresolvedValue = default);
        IReadWriteHandle<T> GetReadWriteHandle<T>(T preresolvedValue = default);
        IWriteHandle<T> GetWriteHandle<T>(T prestagedValue = default);

        #endregion

        #region Layers

        Task<IEnumerable<T>> AllLayersOfType<T>();
        
        #endregion
    }

}
