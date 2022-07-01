#nullable enable

using LionFire.Results;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LionFire.Activities
{

    //public class ProcessActivity : ActivityBase
    //{
    //    public ProcessStartInfo? ProcessStartInfo { get; set; }
    //    public Process? Process { get; set; }
    //}

    public class TaskActivity<TResult> : TaskActivityBase, IActivity<TResult>
    {
        public TaskActivity() { }
        public TaskActivity(Task<TResult> task)
        {
            Task = task;
        }
        public TaskActivity(string name, Task<TResult> task) : base(name)
{
            Task = task;
        }

        public Task<TResult>? Task { get; set; }
        //Task? IActivity.Task { get => this.Task;  }

        protected override IAsyncResult? AsyncResult => Task;


        public override void OnCompleted(Action continuation) => Task?.ContinueWith(_ => continuation());
        public override void GetResult() { Task?.Wait(); }
        public TaskAwaiter<TResult> GetAwaiter() => Task?.GetAwaiter() ?? default;
        TaskAwaiter IActivity.GetAwaiter() => (Task as Task)?.GetAwaiter() ?? default;
        public IActivity<TResult> ConfigureAwait(bool arg1) => this; // TODO

        Task? IActivity.Task => Task;

        IActivity IActivity.ConfigureAwait(bool arg1)
        {
            ConfigureAwait(arg1);
            return this;
        }
    }

    public class TaskActivity : TaskActivityBase, IActivity
    {
        public TaskActivity() { }
        public TaskActivity(Task task)
        {
            Task = task;
        }
        public TaskActivity(string name, Task task) : base(name)
        {
            Task = task;
        }

        public Task? Task { get; set; }
        protected override IAsyncResult? AsyncResult => Task;

        public override void OnCompleted(Action continuation) => Task?.ContinueWith(_ => continuation());
        public override void GetResult() { Task?.Wait(); }
        public TaskAwaiter GetAwaiter() => (Task ?? Task.CompletedTask).GetAwaiter();

        public IActivity ConfigureAwait(bool arg1) => this;

    }

    public abstract class TaskActivityBase : ActivityBase
    {
        public TaskActivityBase() { }
        public TaskActivityBase(string name)
        {
            Name = name;
        }
                
        

        public override string ToString() => Name ?? Key.ToString();

        
    }

}