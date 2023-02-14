#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CircularBuffer;
using LionFire.Activities;
using Microsoft.AspNetCore.Components;

namespace LionFire.Blazor.Components.Terminal
{
    public partial class OutputPane
    {
        [Parameter]
        public Guid Key { get; set; }

        public IActivity? Activity { get; set; }

        protected override Task OnInitializedAsync()
        {
            if(Key != default)
            {
                ActivitiesTracker.TryGetValue(Key, out var activity);
                Activity = activity;
            }
            return base.OnInitializedAsync();
        }

        public IEnumerable<string>? Lines => Activity?.Lines();

    }
}