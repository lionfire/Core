#if duplicate

using LionFire.Structures;
using LionFire.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LionFire.UI
{
    public class ReadOnlyCollectionAdapter<TViewModel, TModel> : CollectionAdapterBase<TViewModel, TModel>
        where TViewModel : class
    {
        ///// <summary>
        ///// Constructor
        ///// </summary>
        ///// <param name="models">List of models to synch with</param>
        ///// <param name="viewModelProvider"></param>
        ///// <param name="context"></param>
        ///// <param name="autoFetch">
        ///// Determines whether the collection of ViewModels should be fetched from the model collection on construction
        ///// </param>
        public ReadOnlyCollectionAdapter(IEnumerable<TModel> models, ObjectTranslator<TModel, TViewModel> viewModelProvider = null, object context = null, bool autoFetch = true, IDispatcher dispatcher = null) : base(models, viewModelProvider, context, autoFetch)
        {
        }
    }
}


using LionFire.Structures;
using LionFire.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LionFire.UI
{
    public class ReadOnlyCollectionAdapter<TViewModel, TModel> : CollectionAdapterBase<TViewModel, TModel>
        where TViewModel : class
    {
        ///// <summary>
        ///// Constructor
        ///// </summary>
        ///// <param name="models">List of models to synch with</param>
        ///// <param name="viewModelProvider"></param>
        ///// <param name="context"></param>
        ///// <param name="autoFetch">
        ///// Determines whether the collection of ViewModels should be fetched from the model collection on construction
        ///// </param>
        public ReadOnlyCollectionAdapter(IEnumerable<TModel> models, ObjectTranslator<TModel, TViewModel> viewModelProvider = null, object context = null, bool autoFetch = true, IDispatcher dispatcher = null) : base(models, viewModelProvider, context, autoFetch)
        {
        }
    }
}

#endif