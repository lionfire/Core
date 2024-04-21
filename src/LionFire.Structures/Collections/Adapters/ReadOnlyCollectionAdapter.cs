#define SourceToTargetMap_off
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using LionFire.Instantiating;
using LionFire.Structures;
using LionFire.Threading;

namespace LionFire.Collections
{
    public class ReadOnlyCollectionAdapter<TTarget, TSource> : CollectionAdapterBase<TTarget, TSource>, IReadOnlyCollectionAdapter<TTarget>
           where TTarget : class // , IInstanceForSettable<TSource>
        where TSource : class
    {
        public bool IsReadOnly => true;

        ///// <summary>
        ///// Constructor
        ///// </summary>
        ///// <param name="models">List of models to synch with</param>
        ///// <param name="viewModelProvider"></param>
        ///// <param name="context"></param>
        ///// <param name="autoFetch">
        ///// Determines whether the collection of ViewModels should be
        ///// fetched from the model collection on construction
        ///// </param>
        public ReadOnlyCollectionAdapter(IEnumerable<TSource> models, ObjectTranslator targetProvider = null, object context = null, bool autoFetch = true, IDispatcher dispatcher = null)
            : base(models, targetProvider, context, autoFetch, dispatcher)
        {
        }
    }
}
