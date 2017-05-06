using LionFire.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using LionFire.Reactive;
using LionFire.Reactive.Subjects;
using System.ComponentModel;
using LionFire.Structures;

namespace LionFire.Execution.Executables
{
    //public class ExecutionStateMachine : ExecutableBase
    //{
    //    public object Owner { get; protected set; }
    //}

    public class ExecutableBase : NotifyPropertyChangedBase, IExecutable
    {
        //#region ExecutionState

        //public IBehaviorObservable<ExecutionState> State
        //{
        //    get
        //    {
        //        return executionState;
        //    }
        //}
        //private BehaviorObservable<ExecutionState> executionState = new BehaviorObservable<ExecutionState>();

        //#endregion

        #region State

        public ExecutionState State
        {
            get { return state; }
            protected set
            {
                if (state == value) return;
                state = value;
                StateChangedToFor?.Invoke(state, this);
            }
        }
        private ExecutionState state;

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
