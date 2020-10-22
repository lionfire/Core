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
    public class Conductor : IConductor
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
        public event Action<IConductor> Closed;

        #endregion

        #region State

        #region Presenters

        System.Collections.Generic.IReadOnlyDictionary<string, IPresenter> IConductor.Presenters => Presenters;
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

        #region (Public) Methods

        public Task Close(bool force = false)
        {
            #region Closing event (with cancel)

            var cea = new CancelableEventArgs();
            Closing?.InvokeCancelable(cea);
            if (cea.CancelRequested) { return; }

            #endregion

            var presenters = Presenters.Values.ToArray();
            Presenters.Clear();
            foreach (IPresenter presenter in presenters)
            {
                if (presenter == MainPresenter) continue; // Close MainPresenter last
                try
                {
                    presenter.Close();
                }
                catch (Exception ex)
                {
                    l.Error("Exception when closing child presenter", ex);
                }
            }
            if (MainPresenter != null)
            {
                try
                {
                    MainPresenter.Close();
                }
                catch (Exception ex)
                {
                    // TOALERT
                    l.Error("Exception when closing main presenter", ex);
                }
            }

            OnClosed();
            return Task.CompletedTask;
        }

        #endregion

        #region (Protected) Methods

        protected virtual IPresenter CreatePresenter(Type type, string name = null) => (IPresenter)ActivatorUtilities.CreateInstance(ServiceProvider, type, this, name);

        #endregion

        #region Event Handling

        #region Closing

        protected virtual void OnClosed()
        {
            // Idea: If this is the main shell (probably is), also try to CloseMainWindow:
            //try
            //{
            //    System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            //}
            //catch // EMPTYCATCH
            //{
            //}
            //    WindowsSettings.Save(); // TODO - make autosaved

            Closed?.Invoke(this);
        }

        protected virtual void OnPresenterDisposing(object obj)
        {
            var presenter = (IPresenter)obj;
            Presenters.Remove(presenter.Key);
        }

        protected virtual void OnMainPresenterDisposing(object obj) => OnPresenterDisposing(obj);

        protected virtual bool RaiseCanceling()
        {
            var cea = new CancelableEventArgs();
            Closing?.InvokeCancelable(cea);
            return !cea.CancelRequested;
        }

        protected virtual void OnSelfInitiatedClose(bool cancelable = true)
        {
            var canceled = RaiseCanceling();
            if (cancelable && canceled) return;

            OnClosed();
        }
        
        #endregion

        #endregion

        #region Misc

        protected static ILogger l = Log.Get();

        #endregion
    }

}

