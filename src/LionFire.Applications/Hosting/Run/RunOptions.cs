using System;
using System.Threading.Tasks;
using System.Threading;

namespace LionFire.Hosting
{
    public class RunOptions
    {
        public RunOptions() { }
        public RunOptions(Func<IServiceProvider, CancellationToken, Task> action) { this.Action = action; }
        public RunOptions(Func<Task> action) { this.Action = (services, stopping) => action(); }
        public RunOptions(Func<IServiceProvider, Task> action) { this.Action = (services, stopping) => action(services); }
        public Func<IServiceProvider, CancellationToken, Task> Action { get; set; }
    }
}
