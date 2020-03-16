// TODO: IOC
//#define SPLASH_TEST
#if !UNITY
#define NAppUpdater
#else
#endif

using System;
using System.Collections.Generic;
using LionFire.Shell;
using LionFire.ObjectBus;
using System.ComponentModel;
using LionFire.Services;
using LionFire.Types;
using Caliburn.Micro;
using LionFire.MultiTyping;
using LionFire.Persistence;

namespace LionFire.Applications
{
    // ENH: Make this a multityped?
    public interface ILionFireApp : INotifyClosing, INotifyPropertyChanged
        , IMultiTyped
         , IHasService
    {
        LionFireAppCapabilities Capabilities { get; }

        ILionFireShell Shell { get; set; }

        IEnumerable<IReadWriteHandle> SettingsObjects { get; }

        string TryRegister(string key); // MOVE to feature interface

        #region Splash

        string SplashMessage { get; set; }
        double SplashProgress { get; set; }

        #endregion

        #region Lifecycle

        void Start();
        void ForceStop(bool tryCleanForceStop = false);
        event Action AppStarted;

        event Action<CancelableEventArgs> Closing;

        #endregion

        AppState AppState { get; }
        event Action<AppState, AppState> AppStateChangedFromTo;

        void RunCommandLineService(); // Move to extension method?


        DateTime StartTime { get; }

        IEventAggregator EventAggregator { get; set; }
    }
}

