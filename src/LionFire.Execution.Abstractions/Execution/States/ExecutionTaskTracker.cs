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
        ConcurrentDictionary<KeyValuePair<object, ExecutionStateEx>, CancellationTokenSource> tokens = new ConcurrentDictionary<KeyValuePair<object, ExecutionStateEx>, CancellationTokenSource>();

        public CancellationToken OnEnteringState(IExecutableEx executable, ExecutionStateEx state)
        {
            var key = new KeyValuePair<object, ExecutionStateEx>(executable, state);
            var cts = tokens.AddOrUpdate(key, _ => new CancellationTokenSource(), (x, y) => new CancellationTokenSource());
            return cts.Token;
        }
    }

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
