using LionFire.Structures;
using System.Threading;


namespace LionFire.Execution
{
    public static class TaskTrackerExtensions
    {
        public static CancellationToken OnEnteringState(this IExecutableEx executable, ExecutionStateEx state)
        {
            var tt = ManualSingleton<TaskTracker>.Instance;
            if (tt == null) return default(CancellationToken);
            return tt.OnEnteringState(executable, state);
        }
    }
}
