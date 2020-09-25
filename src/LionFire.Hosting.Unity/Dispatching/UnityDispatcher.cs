#pragma warning disable IDE0051 // Remove unused private members
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;

namespace LionFire.Dispatching
{

    // Based on https://gist.github.com/heshuimu/f63cd9126117afc4004be37b19fa1c09

    /// <summary>
    /// Do not add this.  It is added to a new GameObject AfterSceneLoad (see CreateDispatcher())
    /// </summary>
    public class UnityDispatcher : MonoBehaviour
    {
        #region Configuration

        /// <summary>
        /// Don't execute any new actions if more than this many milliseconds has elapsed since the start of executing the first action.
        /// </summary>
        public static double MaxMillisecondsPerWindow => 3;

        #endregion

        #region (Static)

        public static UnityDispatcher Instance { get; private set; }

        #region State

        private static readonly ConcurrentQueue<Action> pendingActions = new ConcurrentQueue<Action>();
        private static readonly Stopwatch windowTimeStopwatch = new Stopwatch();

        #endregion

        #region Initialization

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void CreateDispatcher()
        {
            if (Instance == null)
            {
                UnityDispatcher dispatcher = FindObjectOfType<UnityDispatcher>() ?? new GameObject("Unity Thread Dispatcher").AddComponent<UnityDispatcher>();
                DontDestroyOnLoad(dispatcher);
                Instance = dispatcher;
            }
        }

        #endregion

        #region (Public) Execute Methods

        public static Task<TResult> Execute<TResult>(Func<TResult> func)
        {
            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();

            void InternalAction()
            {
                try
                {
                    TResult returnValue = func();
                    tcs.SetResult(returnValue);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            }

            pendingActions.Enqueue(InternalAction);
            return tcs.Task;
        }

        public static Task Execute<TArg>(Action<TArg> action, TArg arg1)
            => Execute(
                () => {
                    action(arg1);
                    return true;
                }
            );

        public static Task Execute<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
            => Execute(
                () => {
                    action(arg1, arg2);
                    return true;
                }
            );


        public static Task<R> Execute<T1, R>(Func<T1, R> func, T1 arg1) => Execute(() => func(arg1));

        public static Task<R> Execute<T1, T2, R>(Func<T1, T2, R> func, T1 arg1, T2 arg2) => Execute(() => func(arg1, arg2));

        public static Task Execute(Action action)
            => Execute(
                () => {
                    action();
                    return true;
                }
            );


        #endregion

        #endregion

        #region Worker

        private void Update()
        {
            windowTimeStopwatch.Restart();

            while (pendingActions.Count != 0 && windowTimeStopwatch.Elapsed.TotalMilliseconds < MaxMillisecondsPerWindow)
            {
                if (pendingActions.TryDequeue(out System.Action action))
                {
                    action();
                }
            }
        }
        
        #endregion
    }



#if OLD
    // Based on https://www.noesisengine.com/forums/viewtopic.php?f=3&t=1659&p=9570&hilit=dispatcher+return#p9570
        private IEnumerator ActionWrapper(Action action)
        {
            action();
            yield return null;
        }

        public void Invoke(IEnumerator action) => executionQueue.Enqueue(() => StartCoroutine(action));
#endif
}
