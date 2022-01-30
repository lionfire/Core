#nullable enable

using LionFire.Results;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Activities
{
    public class ActivityBase : IKeyable<Guid>
    {
        public Guid Key { get; set; }
        public string? Name { get; set; }

        public string? Description { get; set; }
    }

    public class TaskActivity : ActivityBase, IActivity
    {
        public List<TaskActivity>? ContinueWith { get; set; }

        public ActivityStatus? Status { get; set; }
        public bool Finished { get; set; }
        public Task? Task { get; set; }

        public bool IsCompleted => Task?.IsCompleted ?? false;
        public void OnCompleted(Action continuation) => Task?.ContinueWith(_ => continuation());
        public void GetResult() { Task?.Wait(); }

        public override string ToString() => Name ?? Key.ToString();
    }

}