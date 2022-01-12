#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Activities
{
    public class Activity : IActivity
    {
        public string? Name { get; set; }
        public List<Activity>? ContinueWith { get; set; }

        public bool Finished { get; set; }
        public Task? Task { get;  set; }
        public ActivityStatus? Status { get; set; }
        public Guid Key { get;  set; }
        public string? Description { get;  set; }
    }

}