#nullable enable
using LionFire.Dependencies;
using MediatR;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Activities
{
    public  class NewActivity : INotification
    {
        public IActivity Activity { get; set; }
    }

    public class ActivityNotificationHanlder : INotificationHandler<NewActivity>
    {
        public Task Handle(NewActivity newActivity, CancellationToken cancellationToken)
        {
            var a = DependencyContext.Current.GetService<ActivitiesTracker>();
            if(a != null)
            {
                a.Add(newActivity.Activity);
            }
            return Task.CompletedTask;
        }
    }

}