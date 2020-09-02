using Caliburn.Micro;
using LionFire.Applications;
using LionFire.Shell;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY
using UnityEngine;
#endif
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire
{

#if UNITY
    public class AppWaiter : MonoBehaviour
    {
        public Action FinishAction { get; set; }

        public IEnumerable WaitForApp
        {
            get
            {
                int countdown = 5;
                while (LionFireApp.Instance == null || !LionFireApp.Instance.IsInitialized)
                {
//#if UNITY
//                    var obj = new GameObject();
//                    obj.name = "LionFireShell";
//                    obj.AddComponent(LionFireShell.ShellBehaviorType);
//#else
//                        throw new Exception("Please create the LionFireApp.");
//#endif

                    //if (countdown-- <= 0) {
                    //    //UnityEngine.Debug.Log("(Timed out waiting for App to start)");
                    //    yield break;
                    //}
                    UnityEngine.Debug.Log("(Waiting for App to start)");

                    yield return null;
                }
#if UNITY
                GameObject.Destroy(GameObject.Find("AppWaiter")); // TODO: automate the creation of the GameObject for Unity, with the hardcoded name
                UnityEngine.Debug.Log("(Waiting for App to start: finished!)");
#endif
                if (FinishAction != null) { FinishAction(); }
                yield break;
            }
        }
    }
#endif

#if UNUSED
    public static class LionFireShell
    {
#region Instance

        public static ILionFireShell Instance
        {
            get { return instance; }
            set
            {
                if (instance == value) return;
                if (instance != default(LionFireShell)) throw new AlreadySetException();
                instance = value;
            }
        }
        private static ILionFireShell instance;

#endregion

#if UNITY
        public static Type ShellBehaviorType { get; set; }
#endif
        public static void StartAndWait(Action onAppReady = null) // MOVE to LionFire.Utility
        {
#if UNITY
            UnityEngine.Debug.Log("StartAndWait");
            if (LionFireApp.Instance == null || !LionFireApp.Instance.IsInitialized)
            {
                if (LionFireApp.Instance == null)
                {
                    UnityEngine.Debug.Log("Creating new " + ShellBehaviorType.Name);
                    var app = new GameObject();
                    app.name = "LionFireShell";
                    var c = (Shell.Unity.UnityNoesisGuiShellBehaviour)app.AddComponent(ShellBehaviorType);
                    UnityEngine.Debug.Log("Initting app");
                    c.Init();
                    //c.Awake();
                    //UnityEngine.Debug.Log("Running app");
                    //        c.Run();
                }

                if (LionFireApp.Instance != null && !LionFireApp.Instance.IsInitialized)
                {
                    UnityEngine.Debug.LogWarning("!LionFireApp.Instance.IsInitialized after Awake()");
                }

                //{
                //    UnityEngine.Debug.Log("Waiting for App to start... (" + (LionFireApp.Instance == null) + ") " + System.Reflection.Assembly.GetEntryAssembly());
                //    var w = new GameObject();
                //    w.name = "AppWaiter";
                //    var c = (AppWaiter)w.AddComponent(typeof(AppWaiter));
                //    c.FinishAction = onAppReady;
                //    c.StartCoroutine(c.WaitForApp.GetEnumerator());
                //}
            }
            else
            {
                onAppReady();
            }

            if (onAppReady != null) { onAppReady(); }
#else
            if (onAppReady != null) { onAppReady(); }
#endif

        }
    }
#endif
}

namespace LionFire.Shell
{
    public class LionFireShell : ILionFireShell
    {
        public virtual IShellContentPresenter MainPresenter => null;

        public event Action Closing;
        public event Action Closed;

        public virtual void Close()
        {
            if (MainPresenter != null)
            {
                MainPresenter.Close();
            }
            else
            {
                System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            }
        }

        public Task StartAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task StopAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
        public bool AskUserToUpdate() => throw new NotImplementedException();
        public bool AskUserToContinueOnException(Exception exception) => throw new NotImplementedException();
        public Task Stop(CancellationToken? cancellationToken = null) => throw new NotImplementedException();

        public bool IsEditingText { get; set; }
        //public abstract IEventAggregator EventAggregator { get; set; }

        //    #region Events

        //    public override IEventAggregator EventAggregator
        //    {
        //        get { return eventAggregator; }
        //        set { eventAggregator = value; }
        //    }
        //    private IEventAggregator eventAggregator = new EventAggregator();

        //    #endregion

        //    public event Action Closing;
        //    public event Action Closed;
        //}
    }
}
