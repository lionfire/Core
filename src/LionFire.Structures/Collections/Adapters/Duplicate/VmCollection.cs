#if duplicate
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
    public class VmCollection<TViewModel, TModel> : VmCollectionBase<TViewModel, TModel>
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
        public VmCollection(ICollection<TModel> models, ObjectTranslator<TModel, TViewModel> viewModelProvider = null, object context = null, bool autoFetch = true, IDispatcher dispatcher = null, bool useApplicationDispatcher = true)
        {
            _models = models;
            _readOnlyModels = models;
            _context = context;

            _viewModelProvider = viewModelProvider;

            CollectionChanged += ViewModelCollectionChanged;

            // If model collection is observable register change handling for synchronization from Models to ViewModels
            if (models is ObservableCollection<TModel>)
            {
                var observableModels = models as ObservableCollection<TModel>;
                observableModels.CollectionChanged += ModelCollectionChanged;
            }
            else if (models is IEnumerable<TModel> && models is INotifyCollectionChanged observableModels)
            {
                observableModels.CollectionChanged += ModelCollectionChanged;
            }

            // Fecth ViewModels
            if (autoFetch) FetchFromModels();
        }

#endregion
        
    }
}

#endif