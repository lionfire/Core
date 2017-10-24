using LionFire.Collections.Concurrent;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Execution.Jobs
{
    public class BlockerCollection
    {
        //public ValidationContext DelayBlockers { get; private set; } = new ValidationContext();

        private object _lock = new object();
        //public ConcurrentQueue<Task> BlockingTasks { get; private set; } = new ConcurrentQueue<Task>(); 
        //public ImmutableArray<Task> BlockingTasks { get; set; } // FUTURE: Use this instead of blockignqueues?
        public ConcurrentHashSet<object> Blockers { get; private set; }

        public ManualResetEvent ResetEvent { get; private set; } = new ManualResetEvent(false);

        public int Count => Blockers == null ? 0 : Blockers.Count;

        public void Add(object obj)
        {
            lock (_lock)
            {
                if (Blockers == null) Blockers = new ConcurrentHashSet<object>();
                Blockers.Add(obj);
            }
        }
        public void Remove(object obj)
        {
            lock (_lock)
            {
                Blockers.Remove(obj);
                if (Blockers.Count == 0) ResetEvent.Set();
                Unblocked?.Invoke();
            }
        }

        public event Action Unblocked;

        public async Task Wait()
        {
            lock (_lock)
            {
                if (Blockers == null || Blockers.Count == 0) return;
            }
            await Task.Run(() =>
            {
                ManualResetEvent re;
                do
                {
                    lock (_lock)
                    {
                        if (Blockers == null || Blockers.Count == 0) break;
                    }
                    re = ResetEvent;
                }
                while (re != null && re.WaitOne());
            });
        }
    }
}
