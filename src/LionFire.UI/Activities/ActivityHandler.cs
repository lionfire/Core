#nullable enable
using LionFire.Dependencies;
using LionFire.Structures;
using System.Diagnostics;

namespace LionFire.Activities
{
    public class ActivityHandler : IHandler<NewActivity>
    {
        public ActivityHandler()
        {
            Debug.WriteLine("ActivityHandler");
        }

        public void Handle(NewActivity message)
        {
            Debug.WriteLine("ActivityHandler handle todo");

            var tracker = DependencyContext.Current.GetService<IActivitiesTracker>();
            if (tracker != null ) { tracker.Add(message.Activity); }
        }
    }

}