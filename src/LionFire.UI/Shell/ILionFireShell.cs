using LionFire.Applications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace LionFire.Shell
{

    public interface ILionFireShell
    {
        #region AutoUpdate

        /// <summary>
        /// If returns false, suspend App initialization.  Shell must call LionFireApp.ResumeInit()
        /// </summary>
        /// <returns>false if the updater needs to restart.  In this case, the app initialization
        /// should cease.</returns>
        bool AskUserToUpdate();

        #endregion

        #region State

        /// <summary>
        /// Set to true by applications when keyboard is receiving text input.  This can be useful in determining whether certain keyboard events and hotkeys should be disabled while the user is typing.
        /// </summary>
        bool IsEditingText { get; set; }

        #endregion

        #region Invoke

        void Invoke(Action action);
        object Invoke(Func<object> func);
        void BeginInvoke(Action action);

        #endregion

        #region Presenter

#if TOPORT
        IShellContentPresenter MainPresenter { get; }
#endif
        #endregion

        #region Exceptions

        bool AskUserToContinueOnException(Exception exception);

        #endregion

        #region Ontology
#if TOPORT
        ILionFireApp App { get; }
#endif
        #endregion


        #region Info
#if TOPORT
        LionFireAppCapabilities Capabilities { get; }
#endif
        bool ProvidesRunLoop { get; }

        #endregion

        #region Events

        IEventAggregator EventAggregator { get; set; }

        #endregion

        #region Methods

        void Close();

        #endregion
    }
}
