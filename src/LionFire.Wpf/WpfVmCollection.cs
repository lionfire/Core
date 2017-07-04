using System.Collections.Generic;
using System.Windows.Threading;

namespace LionFire.UI.Wpf
{
    //public interface IViewModelProvider<T> : IViewModelProvider
    //    where T : IViewModel
    //{

    //}

        //public class WpfReadOnlyVmCollection<> 

    public class WpfVmCollection<TViewModel, TModel> : VmCollection<TViewModel, TModel>
        where TViewModel : class, IViewModel
        where TModel : class
    {

        public WpfVmCollection(IEnumerable<TModel> readOnlyModels, IViewModelProvider viewModelProvider = null, object context = null, bool autoFetch = true, Dispatcher dispatcher = null, bool useApplicationDispatcher = true)
            : base(readOnlyModels, viewModelProvider, context, autoFetch, dispatcher.ToIDispatcher(), useApplicationDispatcher)
        {
        }
        public WpfVmCollection(ICollection<TModel> models, IViewModelProvider viewModelProvider = null, object context = null, bool autoFetch = true, Dispatcher dispatcher = null, bool useApplicationDispatcher = true)
            : base(models, viewModelProvider, context, autoFetch, dispatcher.ToIDispatcher(), useApplicationDispatcher)
        {
        }
    }

}
