using System.Collections.Generic;
using System.Windows.Threading;
using LionFire.Collections;
using LionFire.Instantiating;
using LionFire.Structures;

namespace LionFire.UI.Wpf
{
    public class WpfVmCollection<TViewModel, TModel> : CollectionAdapter<TViewModel, TModel> // RENAME to WpfCollectionAdapter
        where TViewModel : class, IViewModel, IInstanceFor<TModel>

        where TModel : class
    {
        public WpfVmCollection(ICollection<TModel> models, ObjectTranslator viewModelProvider = null, object context = null, bool autoFetch = true, Dispatcher dispatcher = null, bool useApplicationDispatcher = true)
            : base(models, viewModelProvider, context, autoFetch, dispatcher.ToIDispatcher(), useApplicationDispatcher)
        {
        }
    }
    public class ReadOnlyWpfVmCollection<TViewModel, TModel> : ReadOnlyCollectionAdapter<TViewModel, TModel> // RENAME to WpfReadonlyCollectionAdapter
    where TViewModel : class, IViewModel, IInstanceFor<TModel>
    where TModel : class
    {

        public ReadOnlyWpfVmCollection(IEnumerable<TModel> readOnlyModels, ObjectTranslator viewModelProvider = null, object context = null, bool autoFetch = true, Dispatcher dispatcher = null)
            : base(readOnlyModels, viewModelProvider, context, autoFetch, dispatcher.ToIDispatcher() )
        {
        }
   
    }

}
