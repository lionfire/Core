#define SourceToTargetMap_off
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using LionFire.Instantiating;
using LionFire.Structures;
using LionFire.Threading;

namespace LionFire.Collections
{
    public abstract class CollectionAdapterBase<TViewModel, TModel> : ObservableCollection<TViewModel>
        where TViewModel : class//, IInstanceFor<TSource>
        where TModel : class
    {
        // TODO: Contain ObservableCollection (and provide read-only access for ReadOnlyCollectionAdapter) instead of having it as a base type
        //protected ObservableCollection<TViewModel> collection = new ObservableCollection<TViewModel>();

        public TViewModel[] ToArray()
        {
            return this.ToArray();
        }

        protected readonly IEnumerable<TModel> ReadOnlySources;
        protected readonly object context;
        protected bool syncDisabled;
        //protected readonly ObjectTranslator<TSource, TViewModel> targetProvider;
        protected readonly ObjectTranslator targetProvider;
        protected readonly IDispatcher dispatcher; // TODO: Use this where required in this class

        // FUTURE: Reintroduce class IViewModel<T> { T Model{get;} } in a more generic way?  Such as ITemplate/ITemplateInstance.  And avoid this map if such interfaces are provided.
        // FUTURE: Support the map, since O(n) search for removals 
        //Dictionary<TSource, TViewModel> sourceToTargetMap;

        public Type InstanceType => typeof(TViewModel);
        public Type SourceType => typeof(TModel);

        public CollectionAdapterBase(IEnumerable<TModel> readOnlySources, ObjectTranslator targetProvider = null, object context = null, bool autoFetch = true, IDispatcher dispatcher = null)
        {
            this.ReadOnlySources = readOnlySources;

            //if (typeof(TSource).IsAssignableFrom(IForInstanceOf<TViewModel>))
            //{
            //    sourceToTargetMap = null;
            //}
            //else
            //{
            //    sourceToTargetMap = new Dictionary<TSource, TViewModel>();
            //}

            this.targetProvider = targetProvider;
            this.context = context;
            this.dispatcher = dispatcher;

            // If model collection is observable register change handling for synchronization from Models to ViewModels
            if (ReadOnlySources is ObservableCollection<TModel> observableCollection)
            {
                observableCollection.CollectionChanged += SourceCollectionChanged;
            }
            else if (ReadOnlySources is INotifyCollectionChanged incc)
            {
                incc.CollectionChanged += SourceCollectionChanged;
            }

            // Fecth ViewModels
            if (autoFetch) FetchFromModels();
        }

        protected void SourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (syncDisabled) return;
            syncDisabled = true;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var m in e.NewItems.OfType<TModel>())
                        this.AddIfNotNull(CreateViewModel(m));
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var m in e.OldItems.OfType<TModel>())
                    {
#if SourceToTargetMap
                        if (sourceToTargetMap.ContainsKey(m))
                        {
                            var target = sourceToTargetMap[m];
                            this.Remove(target);
                        }
#else
                        this.Remove(GetViewModelOfModel(m));
#endif
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    Clear();
                    FetchFromModels();
                    break;
            }

            syncDisabled = false;
        }

        protected void ModelCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (syncDisabled) return;
            syncDisabled = true;

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

            syncDisabled = false;
        }

        /// <summary>
        /// CollectionChanged event of the ViewModelCollection
        /// </summary>
        public override sealed event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { base.CollectionChanged += value; }
            remove { base.CollectionChanged -= value; }
        }

        /// <summary>
        /// Load VM collection from model collection
        /// </summary>
        public void FetchFromModels()
        {
            // Deactivate change pushing
            syncDisabled = true;

            // Clear collection
            Clear();

            // Create and add new VM for each model
            foreach (var model in ReadOnlySources)
                AddForModel(model);

            // Reactivate change pushing
            syncDisabled = false;
        }

        private void AddIfNotNull(TViewModel viewModel)
        {
            if (viewModel != null)
            {
                if (dispatcher != null && dispatcher.IsInvokeRequired)
                {
                    dispatcher.Invoke(() => Add(viewModel));
                }
                else
                {
                    this.Add(viewModel);
                }
            }
        }

        private TViewModel CreateViewModel(TModel source)
        {
            if (targetProvider == null)
            {
                throw new Exception("No ViewModelProvider was provided at create time.  Cannot CreateViewModel.");
            }
            var target = (TViewModel) targetProvider(source, context);
#if SourceToTargetMap
            //this.sourceToTargetMap.Add(source, target);
#endif
            return target;
        }

        /// <summary>
        /// Adds a new ViewModel for the specified Model instance
        /// </summary>
        /// <param name="model">Model to create ViewModel for</param>
        public void AddForModel(TModel model)
        {
            Add(CreateViewModel(model));
        }

        /// <summary>
        /// Adds a new ViewModel with a new model instance of the specified type,
        /// which is the ModelType or derived from the Model type
        /// </summary>
        /// <typeparam name="TSpecificModel">Type of Model to add ViewModel for</typeparam>
        public void AddNew<TSpecificModel>() where TSpecificModel : TModel, new()
        {
            var m = new TSpecificModel();
            Add(CreateViewModel(m));
        }

        public TViewModel GetViewModelOfModel(TModel model)
        {
            // TODO OPTIMIZE SLOWALGO - O(n) search
            return Items.OfType<IInstanceFor<TModel>>().FirstOrDefault(v => v.Template == model) as TViewModel;
        }

    }
}
