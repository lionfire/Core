using LionFire.Data.Async.Gets;
using LionFire.Shell;
using LionFire.Structures;
using LionFire.UI.Windowing;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.UI.Entities
{
    public class WindowingRoot : WindowCollection, IWindowingRoot
        //, IFactory<>
    {
        //public ICovariantReadOnlyDictionary<string, IUIKeyed> Object => Windows;

        //public ConcurrentDictionary<string, IPresenter> Windows => CovariantChildren;

        //System.Collections.Generic.IReadOnlyDictionary<string, IPresenter> IWindowCollection.Windows => Windows;

#region Dependencies

        public WindowSettings WindowSettings { get; }

#endregion

#region Construction

        public WindowingRoot(IGetter<WindowSettings> windowSettings, IOptionsMonitor<UIOptions> uiOptionsMonitor, IWindowFactory windowFactory)
        {
            Key = "(windowing)";
            WindowSettings = windowSettings.QueryNonDefaultValue(); // WindowSettings should already be resolved as a Hosted Participant that contributes CanStartShell 
            WindowFactory = windowFactory;
        }

#endregion

#region IRootPresenter

        //public IPresenter MainPresenter { get; protected set; }

#endregion
        
    }

    public class WindowCollection : UICollection<IWindow>, IWindowCollection
    {
        public IWindowFactory WindowFactory { get; protected set; }

        public virtual Task<IWindow> Create(string windowName, object context = null, IDictionary<string, object> settings = null)
        {
            if (WindowFactory == null) { throw new NotSupportedException("No WindowFactory specified"); }
            return WindowFactory.Create(windowName, context, settings);
        }
        //Task<IWindow> IWindowCollection.Create(string windowName, object context, IDictionary<string, object> settings) => throw new System.NotImplementedException();
    }

#if FUTURE
    public interface IChildCreator<TChild> // add instantiation type?
        where TChild : IUIObject
    {
        TChild Create();
    }

    public interface IChildCreator<TChild, TInstantiation>
        where TChild : IUIObject
    {
        TChild Create(TInstantiation parameters);
    }
#endif

}
