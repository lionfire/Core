using System;
using System.Collections.Generic;
using System.Linq;
using LionFire.Execution;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace LionFire.DependencyInjection.Execution
{
    public class DependencyStatus
    {
        public bool IsSatisfied { get; set; }
    }
    // FUTURE?  Put this inside the class State Machine?
    //public static class DependencyExecutableExtensions
    //{
    //    public static Task<DependencyStatus> WaitForState(this IExecutableEx executable, ExecutionStateEx state, int millisecondsTimeout = 0)
    //    {
    //        //executable.ExecutionStates
    //        throw new NotImplementedException(); // FUTURE
    //    }
    //    public static Task<DependencyStatus> WaitForReady(this IExecutableEx executable, int millisecondsTimeout = 0)
    //    {
    //        return executable.WaitForState(ExecutionStateEx.Ready);
    //    }
    //}
}
