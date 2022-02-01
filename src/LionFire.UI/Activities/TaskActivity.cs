#nullable enable

using LionFire.Results;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LionFire.Activities
{

    //public class ProcessActivity : ActivityBase
    //{
    //    public ProcessStartInfo? ProcessStartInfo { get; set; }

    //    public Process? Process { get; set; }
    //}

    public class TaskActivity : ActivityBase, IActivity
    {

        public ActivityStatus? Status { get; set; }
        public Task? Task { get; set; }

        public bool IsCompleted => Task?.IsCompleted ?? false;
        public void OnCompleted(Action continuation) => Task?.ContinueWith(_ => continuation());
        public void GetResult() { Task?.Wait(); }
        public TaskAwaiter GetAwaiter() => (Task ?? Task.CompletedTask).GetAwaiter();

        public override string ToString() => Name ?? Key.ToString();

        public override IActivity ConfigureAwait(bool arg1) => this; // TODO  - implement this? Need to understand it better
    }

}