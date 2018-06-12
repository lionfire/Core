using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using LionFire.Instantiating;
using LionFire.Structures;
using LionFire.Threading;

namespace LionFire.Collections
{
    /// <summary>
    /// Observable collection of ViewModels that pushes changes to a related collection of models
    /// </summary>
    /// <typeparam name="TViewModel">Type of ViewModels in collection</typeparam>
    /// <typeparam name="TModel">Type of models in underlying collection</typeparam>
    public class CollectionAdapter<TViewModel, TModel> : CollectionAdapterBase<TViewModel, TModel>, ICollectionAdapter<TViewModel>
        where TViewModel : class//, IInstanceFor<TModel>
        where TModel : class
    {
        private readonly ICollection<TModel> model;
        public bool IsReadOnly => false;

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
        public CollectionAdapter(ICollection<TModel> sources, ObjectTranslator targetProvider = null, object context = null, bool autoFetch = true, IDispatcher dispatcher = null, bool useApplicationDispatcher = true) : base(sources, targetProvider, context, autoFetch)
        {
            this.model = sources;

            CollectionChanged += ViewModelCollectionChanged;
        }

        #endregion

        private void ViewModelCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Return if synchronization is internally disabled
            if (syncDisabled) return;

            // Disable synchronization
            syncDisabled = true;

            if (model != null)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (var m in e.NewItems.OfType<IInstanceFor<TModel>>().Select(v => v.Template).OfType<TModel>())
                            model.Add(m);
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        foreach (var m in e.OldItems.OfType<IInstanceFor<TModel>>().Select(v => v.Template).OfType<TModel>())
                            model.Remove(m);
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        model.Clear();
                        foreach (var m in e.NewItems.OfType<IInstanceFor<TModel>>().Select(v => v.Template).OfType<TModel>())
                            model.Add(m);
                        break;
                }
            }

            //Enable synchronization
            syncDisabled = false;
        }
    }
}
