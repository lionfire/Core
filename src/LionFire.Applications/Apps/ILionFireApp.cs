using System;
using System.Collections.Generic;
//using LionFire.Shell;
using LionFire.ObjectBus;
using System.ComponentModel;
using LionFire.Services;
using LionFire.Types;
using LionFire.MultiTyping;
using Microsoft.Extensions.Hosting;

namespace LionFire.Applications
{
    /// <summary>
    /// Skeleton to support non-trivial application
    /// 
    ///  - Launch startup UI
    ///  - Splash screen
    ///  - Auto-update
    ///  
    /// </summary>
    public interface ILionFireApp : 
        //INotifyClosing, 
        INotifyPropertyChanged
        //, IMultiTyped
         //, IHasService
    {
#if false
        //LionFireAppCapabilities Capabilities { get; }

        //ILionFireShell Shell { get; set; }

        //IEnumerable<IReadWriteHandle> SettingsObjects { get; }

        //string TryRegister(string key); // MOVE to licensing key feature interface

        //#region Splash

        //string SplashMessage { get; set; }
        //double SplashProgress { get; set; }

        //#endregion

        #region Lifecycle

        void Start();
        void ForceStop(bool tryCleanForceStop = false);
        event Action AppStarted;

        event Action<CancelableEventArgs> Closing;

        #endregion

        AppState AppState { get; }
        event Action<AppState, AppState> AppStateChangedFromTo;

        //void RunCommandLineService(); // Move to extension method?


        DateTime StartTime { get; }

        //IEventAggregator EventAggregator { get; set; }
#endif
    }
}

#if TOPORT
// Port the lifetime/state management part?
// Otherwise, don't use this class -- register its members with DI on an as-needed basis.

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

namespace LionFire.Applications
{
    // ENH: Make this a multityped?
    public interface ILionFireApp :  INotifyPropertyChanged
#if TOPORT
        INotifyClosing,
        , IExtendableMultiTyped
         , IHasService
#endif
    {
        LionFireAppCapabilities Capabilities { get; }

        ILionFireShell Shell { get; set; }

        IEnumerable<IVobHandle> SettingsObjects { get; }

        string TryRegister(string key); // MOVED to ILicensingInterface

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

#endif
