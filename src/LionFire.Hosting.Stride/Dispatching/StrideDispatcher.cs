using LionFire.Collections.Concurrent;
using LionFire.Dependencies;
using LionFire.Threading;
using Stride.Engine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Dispatching
{
    // Inspired by FrostyFeet https://forums.stride3d.net/t/entity-system-in-2-0/1021/2

    public class StrideDispatcher : AsyncScript, IDispatcher
    {
        #region (Static)

        // TODO FIXME - eliminate static
        public static StrideDispatcher Instance => instance ?? throw new DependencyMissingException(typeof(StrideDispatcher).Name);
        private static StrideDispatcher instance;
        public bool HasInstance => instance != null;

        #endregion

        #region Construction

        int threadId;

        public StrideDispatcher()
        {
            threadId = Thread.CurrentThread.ManagedThreadId;

            // TODO: Guard against multiple?
            instance = this;
        }

        #endregion

        #region State

        private ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();

        #endregion

        #region AsyncScript implementation

        public override async Task Execute()
        {
            while (Game.IsRunning)
            {
                while (queue.TryDequeue(out Action action))
                {
                    try
                    {
                        var context = SynchronizationContext.Current;
                        action.Invoke();
                    }
                    catch (Exception ex)
                    {
                        UnhandledException?.Invoke(this, new DispatcherUnhandledExceptionEventArgs(this, ex));
                    }
                }
                await Script.NextFrame();
            }
        }

        #endregion

        #region IDispatcher

        // TODO: Implement
        // ENH: more advanced scheduling (time quota, prioritization)

        //public bool IsInvokeRequired => Thread.CurrentThread.ManagedThreadId == threadId;
        public bool IsInvokeRequired => true; // TODO: How to detect if invoke is required

        public event EventHandler<DispatcherUnhandledExceptionEventArgs> UnhandledException;

        public Task BeginInvoke(Action action)
        {
            if (IsInvokeRequired)
            {
                queue.Enqueue(action);
            }
            else
            {
                action();
            }
            return Task.CompletedTask;
        }
        public void Invoke(Action action) => BeginInvoke(action);

        #region STUB

        // TODO: Request/response to return a value from the stride game thread
        public Task<object> BeginInvoke(Func<object> func)
        {
            if (IsInvokeRequired)
            {
                queue.Enqueue(() => func());
                return Task.FromResult<object>(null);
            }
            else
            {
                return Task.FromResult(func());
            }
        }
        public object Invoke(Func<object> func) => BeginInvoke(func);

        #endregion

        #endregion

    }
}
