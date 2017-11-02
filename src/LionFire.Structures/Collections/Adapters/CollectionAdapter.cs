#define SourceToTargetMap_off
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using LionFire.Instantiating;
using LionFire.Threading;

namespace LionFire.Collections
{
    /// <summary>
    /// Observable collection of ViewModels that pushes changes to a related collection of models
    /// </summary>
    /// <typeparam name="TTarget">Type of ViewModels in collection</typeparam>
    /// <typeparam name="TSource">Type of models in underlying collection</typeparam>
    public class CollectionAdapter<TTarget, TSource> : CollectionAdapterBase<TTarget, TSource>, ICollectionAdapter<TTarget>
        where TTarget : class, IInstanceFor<TSource>
        where TSource : class
    {
        private readonly ICollection<TSource> sources;
        protected override IEnumerable<TSource> ReadOnlySources => sources;

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="models">List of models to synch with</param>
        /// <param name="viewModelProvider"></param>
        /// <param name="context"></param>
        /// <param name="autoFetch">
        /// Determines whether the collection of ViewModels should be
        /// fetched from the model collection on construction
        /// </param>
        public CollectionAdapter(ICollection<TSource> sources, ObjectTranslator<TSource, TTarget> targetProvider = null, object context = null, bool autoFetch = true, IDispatcher dispatcher = null, bool useApplicationDispatcher = true) : base(targetProvider, context, autoFetch)
        {
            this.sources = sources;

            // Register change handling for synchronization from ViewModels to Models
            CollectionChanged += ViewModelCollectionChanged;
        }

        #endregion

        private void ViewModelCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Return if synchronization is internally disabled
            if (syncDisabled) return;

            // Disable synchronization
            syncDisabled = true;

            if (sources != null)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (var m in e.NewItems.OfType<IInstanceFor<TSource>>().Select(v => v.Template).OfType<TSource>())
                            sources.Add(m);
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        foreach (var m in e.OldItems.OfType<IInstanceFor<TSource>>().Select(v => v.Template).OfType<TSource>())
                            sources.Remove(m);
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        sources.Clear();
                        foreach (var m in e.NewItems.OfType<IInstanceFor<TSource>>().Select(v => v.Template).OfType<TSource>())
                            sources.Add(m);
                        break;
                }
            }

            //Enable synchronization
            syncDisabled = false;
        }
    }
}
