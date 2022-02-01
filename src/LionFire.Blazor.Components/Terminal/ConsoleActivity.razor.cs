using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CircularBuffer;
using LionFire.Activities;
using Microsoft.AspNetCore.Components;

namespace LionFire.Blazor.Components.Terminal
{
    public partial class ConsoleActivity
    {
        [Parameter]
        public Guid Key { get; set; }

        public IActivity Activity { get; set; }

        protected override Task OnInitializedAsync()
        {
            if(Key != default)
            {
                ActivitiesTracker a;
                a.Activities.TryGetValue(Key, out var activity);
                Activity = activity;
            }
            return base.OnInitializedAsync();
        }

        public CircularBuffer<string> Lines = new CircularBuffer<string>(1000, new[]{});
    }
}