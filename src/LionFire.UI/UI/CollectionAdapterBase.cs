using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using LionFire.Structures;
using LionFire.Threading;

namespace LionFire.UI
{
    public class CollectionAdapterBase<TViewModel, TModel>: ObservableCollection<TViewModel>
        where TViewModel : class
    {
        protected readonly IEnumerable<TModel> _readOnlyModels;

        protected object _context;
        protected bool _syncDisabled;
        protected ObjectTranslator<TModel, TViewModel> _viewModelProvider;
        protected IDispatcher dispatcher; // TODO: Make sure this is used where required 

        public CollectionAdapterBase(IEnumerable<TModel> models, ObjectTranslator<TModel, TViewModel> viewModelProvider = null, object context = null, bool autoFetch = true)
        {
            _readOnlyModels = models;
            _context = context;
            _viewModelProvider = viewModelProvider;
            this.dispatcher = dispatcher;


            // If model collection is observable register change handling for synchronization from Models to ViewModels
            if (_readOnlyModels is ObservableCollection<TModel> observableCollection)
            {
                observableCollection.CollectionChanged += ModelCollectionChanged;
            }
            else if (_readOnlyModels is IEnumerable<TModel> && _readOnlyModels is INotifyCollectionChanged incc)
            {
                incc.CollectionChanged += ModelCollectionChanged;
            }

            if (autoFetch) FetchFromModels();

        }

        #region Change event handlers

        /// <summary>
        /// CollectionChanged event of the ViewModelCollection
        /// </summary>
        public override sealed event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { base.CollectionChanged += value; }
            remove { base.CollectionChanged -= value; }
        }

        protected void ModelCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_syncDisabled) return;
            _syncDisabled = true;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var m in e.NewItems.OfType<TModel>())
                        this.AddIfNotNull(CreateViewModel(m));
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var m in e.OldItems.OfType<TModel>())
                        this.Remove(GetViewModelOfModel(m));
                    break;

                case NotifyCollectionChangedAction.Reset:
                    Clear();
                    FetchFromModels();
                    break;
            }

            _syncDisabled = false;
        }

        #endregion

        #region (Public) Refresh

        /// <summary>
        /// Load VM collection from model collection
        /// </summary>
        public void FetchFromModels()
        {
            // Deactivate change pushing
            _syncDisabled = true;

            // Clear collection
            Clear();

            // Create and add new VM for each model
            foreach (var model in _readOnlyModels)
                AddForModel(model);

            // Reactivate change pushing
            _syncDisabled = false;
        }

        #endregion

        #region (Public) Add


        /// <summary>
        /// Adds a new ViewModel for the specified Model instance
        /// </summary>
        /// <param name="model">Model to create ViewModel for</param>
        protected void AddForModel(TModel model)
        {
            Add(CreateViewModel(model));
        }

        /// <summary>
        /// Adds a new ViewModel with a new model instance of the specified type,
        /// which is the ModelType or derived from the Model type
        /// </summary>
        /// <typeparam name="TSpecificModel">Type of Model to add ViewModel for</typeparam>
        public TViewModel AddNew<TSpecificModel>() where TSpecificModel : TModel, new()
        {
            var m = new TSpecificModel();
            var vm = CreateViewModel(m);
            Add(vm);
            return vm;
        }

        #endregion

        #region (Public) Get adapter/adaptee

        public TViewModel GetViewModelOfModel(TModel model)
        {
            return Items.OfType<IViewModel<TModel>>().FirstOrDefault(v => v.IsViewModelOf(model)) as TViewModel;
        }

        #endregion

        #region (Protected) Translation and helpers

        protected TViewModel CreateViewModel(TModel model)
        {
            if (_viewModelProvider == null)
            {
                throw new Exception("No ViewModelProvider was provided at create time.  Cannot CreateViewModel.");
            }
            return _viewModelProvider.Invoke(model, _context);
        }

        protected void AddIfNotNull(TViewModel viewModel)
        {
            if (viewModel != null)
            {

                if (dispatcher != null && !dispatcher.CheckAccess())
                {
                    dispatcher.Invoke(() => Add(viewModel));
                }
                else
                {
                    this.Add(viewModel);
                }
            }
        }

        #endregion
        
    }
}
