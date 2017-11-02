#define SourceToTargetMap_off
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using LionFire.Instantiating;
using LionFire.Threading;

namespace LionFire.Collections
{
    public class ReadOnlyCollectionAdapter<TTarget, TSource> : CollectionAdapterBase<TTarget, TSource>, IReadOnlyCollectionAdapter<TTarget>
           where TTarget : class, IInstanceFor<TSource>
        where TSource : class
    {
        protected override IEnumerable<TSource> ReadOnlySources => readOnlySources;
        protected readonly IEnumerable<TSource> readOnlySources;

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
        public ReadOnlyCollectionAdapter(IEnumerable<TSource> models, ObjectTranslator<TSource, TTarget> targetProvider = null, object context = null, bool autoFetch = true, IDispatcher dispatcher = null)
            : base(targetProvider, context, autoFetch, dispatcher)
        {
            readOnlySources = models;

            // If model collection is observable register change handling for synchronization from Models to ViewModels
            if (models is ObservableCollection<TSource>)
            {
                var observableModels = models as ObservableCollection<TSource>;
                observableModels.CollectionChanged += ModelCollectionChanged;
            }
            else if (models is IEnumerable<TSource> && models is INotifyCollectionChanged observableModels)
            {
                observableModels.CollectionChanged += ModelCollectionChanged;
            }

            // Fecth ViewModels
            if (autoFetch) FetchFromModels();
        }
    }
}
