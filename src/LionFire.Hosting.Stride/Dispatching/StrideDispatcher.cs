using LionFire.Collections.Concurrent;
using LionFire.Dependencies;
using LionFire.Threading;
using Stride.Engine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

// Inspired by FrostyFeet https://forums.stride3d.net/t/entity-system-in-2-0/1021/2

namespace LionFire.Dispatching
{

    // ENH: Allow queuing actions before this instance is available -- buffer them in another class and hold them until StrideDispatcher arrives
    //public class StrideDispatcherGateway, IDispatcher
    //{

    //}

    public class ExceptionDispatcher : IDispatcher
    {

        public bool IsInvokeRequired => throw new DependencyMissingException();

        public event EventHandler<DispatcherUnhandledExceptionEventArgs> UnhandledException;

        public Task BeginInvoke(Action action) => throw new DependencyMissingException();
        public Task<object> BeginInvoke(Func<object> func) => throw new DependencyMissingException();
        public void Invoke(Action action) => throw new DependencyMissingException();
        public object Invoke(Func<object> func) => throw new DependencyMissingException();
    }

    public class StrideDispatcher : AsyncScript, IDispatcher
    {
        #region (Static)

        public static IDispatcher Instance => instance ?? (IDispatcher) new ExceptionDispatcher();
        private static StrideDispatcher instance;

        #endregion

        #region Construction

        public StrideDispatcher()
        {
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
                while(queue.TryDequeue(out Action action))
                {
                    try
                    {
                        action.Invoke();
                    }
                    catch(Exception ex)
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

        public bool IsInvokeRequired => throw new NotImplementedException();

        public event EventHandler<DispatcherUnhandledExceptionEventArgs> UnhandledException;

        public Task BeginInvoke(Action action)
        {
            queue.Enqueue(action);
            return Task.CompletedTask;
        }
        public Task<object> BeginInvoke(Func<object> func) => throw new NotImplementedException();

        public void Invoke(Action action) => throw new NotImplementedException();
        public object Invoke(Func<object> func) => throw new NotImplementedException();

        #endregion

    }
}
