#if false // OBsolte, use LionFire.Collections.CollectionAdapter and ReadOnlyCollectionAdapter
// Based on http://stackoverflow.com/a/15831128/208304

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Instantiating;
using LionFire.Structures;
using LionFire.Threading;

namespace LionFire.UI
{
    // RENAME to CollectionAdapter

    /// <summary>
    /// Observable collection of ViewModels that pushes changes to a related collection of models
    /// </summary>
    /// <typeparam name="TViewModel">Type of ViewModels in collection</typeparam>
    /// <typeparam name="TModel">Type of models in underlying collection</typeparam>
    public class VmCollection<TViewModel, TModel> : CollectionAdapterBase<TViewModel, TModel>
        where TViewModel : class, IViewModel
        where TModel : class
    {
        private readonly ICollection<TModel> _models;

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
        public VmCollection(ICollection<TModel> models, ObjectTranslator<TModel, TViewModel> viewModelProvider = null, object context = null, bool autoFetch = true, IDispatcher dispatcher = null, bool useApplicationDispatcher = true) : base(models)
        {
            _models = models;
            CollectionChanged += ViewModelCollectionChanged;
        }

        #endregion

        protected void ViewModelCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_syncDisabled) return;
            _syncDisabled = true;

            if (_models != null)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (var m in e.NewItems.OfType<IViewModel>().Select(v => v.Model).OfType<TModel>())
                            _models.Add(m);
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        foreach (var m in e.OldItems.OfType<IViewModel>().Select(v => v.Model).OfType<TModel>())
                            _models.Remove(m);
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        _models.Clear();
                        foreach (var m in e.NewItems.OfType<IViewModel>().Select(v => v.Model).OfType<TModel>())
                            _models.Add(m);
                        break;
                }
            }

            _syncDisabled = false;
        }

    }
}
#endif