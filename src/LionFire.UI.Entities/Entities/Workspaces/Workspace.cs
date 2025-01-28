#nullable enable
using LionFire.Instantiating;
using LionFire.Execution.Executables;
using System.Collections.ObjectModel;
using LionFire.Composables;
using System.Collections.Generic;
using LionFire.Persistence;
using System.Threading.Tasks;
using System;
using LionFire.Data.Async.Gets;
using LionFire.Results;
using LionFire.FlexObjects;
using Swordfish.NET.Collections;
using LionFire.Structures;
using LionFire.Data.Async.Sets;

namespace LionFire.UI.Workspaces;


#if TODO // Maybe
public class Workspace : IWorkspace
{
    #region Key

    [SetOnce]
    public string? Key
    {
        get => key;
        set
        {
            if (key == value) return;
            if (key != default) throw new AlreadySetException();
            key = value;
        }
    }
    private string? key;

    #endregion

    public object? FlexData { get; set; }

    //public Func<object, string> KeyProvider { get; set; } = item => $"{(item ?? throw new ArgumentNullException()).GetType().FullName}:{(item as IKeyed<string>)?.Key ?? Guid.NewGuid().ToString()}";

    //public ConcurrentObservableSortedDictionary<string, object> Items { get; private set; } = new ConcurrentObservableSortedDictionary<string, object>();

    //public void Add(object item)
    //{
    //    Items.Add(KeyProvider(item), item);
    //}

}
#endif

// UNUSED REVIEW - What is this?  Not sure I want templated.  Maybe it is useful in some situations.  Not sure I want TTemplate in here.
// MOVE? Split Caliburn.Micro app stuff into its own DLL?
//#if FUTURE

// Features:
// - Executable initialization
public abstract class WorkspaceOld<TTemplate, TChild> : InitializableExecutableBase, ITemplateInstance<TTemplate>, IComposition, 
    //IWorkspace, 
    //ISets,
    IFlex

    where TTemplate : class, ITemplate
{
    IEnumerable<object> IComposition.Children => Children;

    public ObservableCollection<object> Children { get; private set; } = new ObservableCollection<object>();


    public TTemplate Template
    {
        get => template;
        set
        {
            if (template == value) return;
            var oldValue = template;
            template = value;
            OnTemplateChanged(oldValue, template);
        }
    }
    private TTemplate template;

    object IFlex.FlexData { get; set; }

    protected virtual void OnTemplateChanged(TTemplate oldValue, TTemplate newValue) { }

    public virtual Task<ISetResult<TValue>> Set<TValue>(TValue value)
    {
        throw new NotImplementedException();
    }
}
// TODO: Children Executable Visitor: attach to object, then on onstarting/onstopping, crawl thru the hierarchy
// TODO: Init call a global executablemanager to say that an executable is intializing, to give it a chance to add state listeners
//#endif
