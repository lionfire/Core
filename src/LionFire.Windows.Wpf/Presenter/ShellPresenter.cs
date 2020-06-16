using LionFire.Collections;
using LionFire.UI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace LionFire.Shell.Wpf
{
    public class WpfShellPresenter : IShellPresenter
    {
        #region Shell

        [SetOnce]
        public WpfShell Shell
        {
            get => shell;
            set
            {
                if (shell == value) return;
                if (shell != default) throw new AlreadySetException();
                shell = value;
            }
        }
        private WpfShell shell;

        #endregion

        #region Dependencies

        public IOptionsMonitor<RootInterfaceOptions> RootInterface { get; }

        #endregion

        #region Construction

        public WpfShellPresenter( IOptionsMonitor<RootInterfaceOptions> rootInterface)
        {
            RootInterface = rootInterface;
        }

        #endregion

        #region State

        #region Presenters

        public MultiBindableDictionary<string, ShellContentPresenter> Presenters = new MultiBindableDictionary<string, ShellContentPresenter>();

        #endregion

        #endregion

        #region Properties

        #region Derived

        public bool AnyActive
        {
            get
            {
                foreach (var presenter in Presenters.Values.ToArray())
                {
                    if (presenter.IsActive) return true;
                }
                return false;
            }
        }

        #endregion

        #endregion

        #region (Public) Methods

        public void Close()
        {

            var presenters = Presenters.Values.ToArray();
            Presenters.Clear();
            foreach (ShellContentPresenter scp in presenters)
            {
                try
                {
                    scp.Close();
                }
                catch (Exception ex)
                {
                    l.Error("Exception when closing child presenter", ex);
                }
            }
            //MainPresenter.Close();
        }

        #region Root Views

        /// <summary>
        /// Invoked once at startup to bring up primary views
        /// </summary>
        public void ShowRootViews()
        {
            try
            {
                foreach (var r in RootInterface.CurrentValue.RootInterfaces)
                {
                    Show(r);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            MainPresenter.Show();
        }

        #endregion

        #region UIReference

        public void Show(UIReference reference)
        {
            if (reference.RootViewAction != null)
            {
                reference.RootViewAction();
            }
            else if (reference.RootView != null)
            {
                throw new NotImplementedException(nameof(reference.RootView));
            }
            else if (reference.RootViewType != null)
            {
                ShowControl(reference.RootViewType, WpfShellTabNames.DefaultTabName);
            }
            else if (reference.RootViewModelType != null)
            {
                throw new NotImplementedException("NEXT: View resolver");
                //ShowControl(reference.RootViewType, WpfShellTabNames.DefaultTabName);
            }
            else
            {
                throw new Exception("No default view or view action");
            }
        }

        #endregion

        #region Views

        public object ShowControl(Type type, string tabName = null)
        {
            MethodInfo mi = this.GetType().GetMethod("ShowControl", new Type[] { typeof(string) }).MakeGenericMethod(type);
            return mi.Invoke(this, new object[] { tabName });
        }

        public T ShowControl<T>(string tabName = null) where T : FrameworkElement => MainPresenter.PushTab<T>(tabName);

        //public T GetControl<T>(string tabName = null OLD
        //    //, T frameworkElement = null
        //    , bool showControl = false) where T : FrameworkElement
        //{
        //    var result = MainPresenter.GetControl<T>(tabName
        //        //, frameworkElement
        //        , showControl);
        //}

        //public void AddControl<T>(T controlInstance, string tabName = null) where T : FrameworkElement // UNUSED 
        //{
        //    mainPresenter.AddControl(controlInstance, tabName);
        //}

        #endregion

        #endregion

        #region MainPresenter

        // ENH - eliminate the concept of MainPresenter, and instead use options for special behavior for "main" presenters, or let application name a particular presenter as "main"

        private const string MainPresenterName = "_Main";

        IShellContentPresenter IShellPresenter.MainPresenter => this.MainPresenter;
        public ShellContentPresenter MainPresenter
        {
            get
            {
                if (mainPresenter == null)
                {
                    mainPresenter = new ShellContentPresenter(this, MainPresenterName);
                    //t.SetApartmentState(ApartmentState.STA); // OLD - is this needed for some reason?

                    Presenters.Add(mainPresenter.Name, mainPresenter);
                    mainPresenter.Closed += new Action<ShellContentPresenter>(mainPresenter_Closed);
                    //mainPresenter.Closing += new Action<ShellContentPresenter>(mainPresenter_Closing);
                }
                return mainPresenter;
            }
        }
        private ShellContentPresenter mainPresenter = null;

        //void mainPresenter_Closing(ShellContentPresenter obj)
        //{
        //    WindowsSettings.Save();
        //}

        async void mainPresenter_Closed(ShellContentPresenter obj)
        {
            Presenters.Remove(obj.Name);

            if (Shell.StopOnMainPresenterClose) // TODO - move
            {
                await Shell.StopAsync(default).ConfigureAwait(false);
            }
        }

        #endregion

        // REVIEW
        public Dictionary<string, Type> PresenterTypes = new Dictionary<string, Type>();

        #region Presenters Collection

        // Tag of the the TabItem in a ShellContentPresent is the key.
        // ShellContentPresenters's contain tabItems
        // The App contains potentially several ShellContentPresenters, typically one per monitor

        // Now what about MDI windows?  Do they live on top?  I guess some could, and some could live in the tab.  
        // If they live in the Tab, I guess the tab has a canvas layer.  
        // Or there could be a dock manager!  Same deal.

        public string GetPresenterNameForControl(string controlKey)
        {
            foreach (KeyValuePair<string, ShellContentPresenter> kvp in Presenters)
            {
                ShellContentPresenter scp = kvp.Value;
                if (scp.Contains(controlKey))
                {
                    return kvp.Key;
                }
            }
            return null;
        }

        #endregion


        private static ILogger l = Log.Get();

    }

}
