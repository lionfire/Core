#nullable enable

using LionFire.Structures;
using System;

namespace LionFire.Activities
{
    public class ActivityBase : IKeyable<Guid>
    {
        public Guid Key { get; set; }
        public string? Name { get; set; }

        public object? FlexData { get; set; }

        public string? Description { get; set; }

        //public List<IActivity>? ContinueWith { get; set; }

        public virtual IActivity ConfigureAwait(bool arg1) => (IActivity)this;
    }

}