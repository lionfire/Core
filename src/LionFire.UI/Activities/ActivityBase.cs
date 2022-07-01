#nullable enable

using LionFire.Structures;
using System;

namespace LionFire.Activities
{
    public abstract class ActivityBase : IKeyable<Guid>, IActivityBase
    {
        public Guid Key { get; set; }
        public string? Name { get; set; }

        public object? FlexData { get; set; }

        public string? Description { get; set; }
        //public List<IActivity>? ContinueWith { get; set; }
        

        public bool Foreground { get; set; }
        public ActivityOptions? Options { get; set; }


        public ActivityStatus? Status { get; protected set; }

        protected abstract IAsyncResult? AsyncResult { get; }

        public bool IsCompleted => AsyncResult?.IsCompleted ?? false;

        public abstract void GetResult();


        public abstract void OnCompleted(Action continuation);
        
                

    }

}