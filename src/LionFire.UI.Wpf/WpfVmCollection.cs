using System.Collections.Generic;
using System.Windows.Threading;
using LionFire.Collections;
using LionFire.Instantiating;

namespace LionFire.UI.Wpf
{
    //public interface IViewModelProvider<T> : IViewModelProvider
    //    where T : IViewModel
    //{

    //}

        //public class WpfReadOnlyVmCollection<> 

    public class WpfVmCollection<TViewModel, TModel> : CollectionAdapter<TViewModel, TModel>
        where TViewModel : class, IViewModel, IInstanceFor<TModel>

        where TModel : class
    {
        public WpfVmCollection(ICollection<TModel> models, ObjectTranslator<TModel, TViewModel> viewModelProvider = null, object context = null, bool autoFetch = true, Dispatcher dispatcher = null, bool useApplicationDispatcher = true)
            : base(models, viewModelProvider, context, autoFetch, dispatcher.ToIDispatcher(), useApplicationDispatcher)
        {
        }
    }
    public class ReadOnlyWpfVmCollection<TViewModel, TModel> : ReadOnlyCollectionAdapter<TViewModel, TModel>
    where TViewModel : class, IViewModel, IInstanceFor<TModel>
    where TModel : class
    {

        public ReadOnlyWpfVmCollection(IEnumerable<TModel> readOnlyModels, ObjectTranslator<TModel, TViewModel> viewModelProvider = null, object context = null, bool autoFetch = true, Dispatcher dispatcher = null)
            : base(readOnlyModels, viewModelProvider, context, autoFetch, dispatcher.ToIDispatcher() )
        {
        }
   
    }
}
