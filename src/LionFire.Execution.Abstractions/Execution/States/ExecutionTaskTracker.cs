using LionFire.Structures;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace LionFire.Execution
{
    public class TaskTracker
    {
        ConcurrentDictionary<KeyValuePair<object, ExecutionState>, CancellationTokenSource> tokens = new ConcurrentDictionary<KeyValuePair<object, ExecutionState>, CancellationTokenSource>();

        public CancellationToken OnEnteringState(IExecutable executable, ExecutionState state)
        {
            var key = new KeyValuePair<object, ExecutionState>(executable, state);
            var cts = tokens.AddOrUpdate(key, _ => new CancellationTokenSource(), (x, y) => new CancellationTokenSource());
            return cts.Token;
        }
    }

    public static class TaskTrackerExtensions
    {
        public static CancellationToken OnEnteringState(this IExecutable executable, ExecutionState state)
        {
            var tt = ManualSingleton<TaskTracker>.Instance;
            if (tt == null) return default(CancellationToken);
            return tt.OnEnteringState(executable, state);
        }
    }
}
