using LionFire.Collections;
using LionFire.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Shell.Wpf
{
    //public interface IConductor : IUIContainer, ICloseable
    //{

//    #region Show

//    //Task Show(UIInstantiation instantiation);

//    #endregion

//}

#if false
    public class Conductor : IUIContainer, ICloseable
    {

#region Dependencies

        protected IServiceProvider ServiceProvider { get; }

        protected IDispatcher Dispatcher { get; }

#endregion

#region Construction and Destruction

        public Conductor(IServiceProvider serviceProvider, IDispatcher dispatcher)
        {
            ServiceProvider = serviceProvider;
            Dispatcher = dispatcher;

        }

#endregion

#region Events

        public event Action<CancelableEventArgs> Closing;
        public event Action<object> Closed;

#endregion

#region State

#region Presenters

        System.Collections.Generic.IReadOnlyDictionary<string, IPresenter> IPresenterContainer.Presenters => Presenters;
        protected MultiBindableDictionary<string, IPresenter> Presenters { get; } = new MultiBindableDictionary<string, IPresenter>();

#region MainPresenter

        // ENH - Factor out the concept of MainPresenter, and instead use options and event handlers for special behavior for "main" presenters, or let application name a particular presenter as "main"

        private const string MainPresenterName = WindowProfile.MainName;

        public IPresenter MainPresenter
        {
            get
            {
                if (mainPresenter == null)
                {
                    throw new Exception("Not initialized");
                }
                return mainPresenter;
            }
            set
            {
                if (mainPresenter == value) return;

                if (mainPresenter != null)
                {
                    Presenters.Remove(mainPresenter.Key);
                    if (mainPresenter is INotifyDisposing nd)
                    {
                        nd.Disposing -= new Action<object>(OnMainPresenterDisposing);
                    }
                }

                mainPresenter = value;

                if (mainPresenter != null)
                {
                    Presenters.Add(mainPresenter.Key, mainPresenter);

                    if (mainPresenter is INotifyDisposing nd)
                    {
                        nd.Disposing += new Action<object>(OnMainPresenterDisposing);
                    }
                }
            }
        }
        private IPresenter mainPresenter = null;

#endregion

#region Derived

        public bool AnyActive => Presenters.Values.Where(p => p.IsActive).Any();

#endregion

#endregion

#endregion
   

#region (Protected) Methods

        protected virtual IPresenter CreatePresenter(Type type, string name = null) => (IPresenter)ActivatorUtilities.CreateInstance(ServiceProvider, type, this, name);

#endregion

#region Event Handling

        protected virtual void OnMainPresenterDisposing(object obj) => OnPresenterDisposing(obj);


#endregion

#region Misc

        protected static ILogger l = Log.Get();

#endregion
    }
#endif

}

