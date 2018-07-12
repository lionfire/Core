using System;
using LionFire.Structures;

namespace LionFire.Execution.Executables
{
    //[Obsolete("Use ExecutableBase")] TODO - obsolete this?
    public class ExecutableExBase : NotifyPropertyChangedBase, IExecutableEx
    {
        #region State

        public ExecutionStateEx State
        {
            get { lock (stateLock) return state; }
            protected set
            {
                lock (stateLock)
                {
                    if (state == value) return;
                    state = value;
                }
                StateChangedToFor?.Invoke(value, this);
            }
        }
        private ExecutionStateEx state;

        public bool SetState(ExecutionStateEx from, ExecutionStateEx to)
        {
            lock (stateLock)
            {
                if (state != from) return false;
                State = to;
            }
            return true;
        }
        private object stateLock = new object();

        public event Action<ExecutionStateEx, object> StateChangedToFor;

        #endregion
    }
}
