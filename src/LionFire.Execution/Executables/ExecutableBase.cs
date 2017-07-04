using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using LionFire.Reactive;
using LionFire.Reactive.Subjects;
using System.ComponentModel;
using LionFire.Structures;

namespace LionFire.Execution.Executables
{

    public class ExecutableBase : NotifyPropertyChangedBase, IExecutable
    {

        #region State

        public ExecutionState State
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
        private ExecutionState state;

        public bool SetState(ExecutionState from, ExecutionState to)
        {
            lock (stateLock)
            {
                if (state != from) return false;
                State = to;
            }
            return true;
        }
        private object stateLock = new object();

        public event Action<ExecutionState, IExecutable> StateChangedToFor;

        #endregion

    }


    //// Not Recommended
    //public class DependencyExecutable : ExecutableBase, IHasDependencies, IRequiresServices
    //{
    //    #region Dependencies


    //    public IServiceProvider ServiceProvider {
    //        get { return serviceProvider; }
    //        set {
    //            serviceProvider = value;
    //        }
    //    }
    //    protected IServiceProvider serviceProvider;

    //    #endregion

    //    public UnsatisfiedDependencies UnsatisfiedDependencies {
    //        get {
    //            return unsatisfiedDependencies;
    //        }
    //    }
    //    protected UnsatisfiedDependencies unsatisfiedDependencies = null;

    //    /// <summary>
    //    /// Invokes this.TryResolveDependencies(ref unsatisfiedDependencies, ServiceProvider).  
    //    /// </summary>
    //    /// <returns>True if there are no unresolved dependencies</returns>
    //    protected virtual bool TryResolveDependencies()
    //    {
    //        return this.TryResolveDependencies(ref unsatisfiedDependencies, ServiceProvider);
    //    }
    //}
}
