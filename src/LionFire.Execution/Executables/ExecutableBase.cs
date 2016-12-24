using LionFire.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using LionFire.Reactive;
using LionFire.Reactive.Subjects;

namespace LionFire.Execution.Executables
{
    public class ExecutableBase : IExecutable
    {
        #region ExecutionState

        public IBehaviorObservable<ExecutionState> State {
            get {
                return executionState;
            }
        }
        private BehaviorObservable<ExecutionState> executionState = new BehaviorObservable<ExecutionState>();

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
