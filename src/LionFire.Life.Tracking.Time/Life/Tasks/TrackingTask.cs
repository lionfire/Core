#if UNUSED
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Tracking.Tasks
{
    public class TaskHistoryItem
    {

    }

    public class NotificationStrategyItem
    {
        public List<INotificationDestination> Destinations { get; set; }


    }

    public class NotificationStrategy
    {
        public List<NotificationStrategyItem> NotificationStrategyItems { get; set; }


        public static NotificationStrategy DefaultCriticalMustAck
        {
            get
            {
                var s = new NotificationStrategy();
                s.NotificationStrategyItems = new List<NotificationStrategyItem>{
                new NotificationStrategyItem
                {
                    Destinations = new List<INotificationDestination>{
                        new SmsNotificationDestination { PhoneNumber = "4034670230"},
                        new EmailNotificationDestination { EmailAddress = "jared+notification@thirsk.ca"},
                    }
                },
            };

                return s;
            }
        }
    }

    public class Reminder
    {
        public bool MustAcknowledge { get; set; }

        //public 
    }



    public class SmsNotificationDestination : INotificationDestination
    {
        public string PhoneNumber { get; set; }
    }

    public class EmailNotificationDestination : INotificationDestination
    {
        public string EmailAddress { get; set; }
    }

    public interface INotificationDestination
    {

    }
    public class NotificationDestination
    {

    }

    public class DueDate
    {
        public DateTime Date { get; set; }
    }

    public class EstimatedDurationItem
    {
        public float Probability { get; set; }
        public TimeSpan Duration { get; set; }
    }
    public class EstimatedDuration
    {
        public List<EstimatedDurationItem> Items { get; set; }

        public TimeSpan? EffectiveTimeSpan
        {
            get
            {
                if (TimeSpan.HasValue) { return TimeSpan.Value; }
                else
                {
                    return null;
                    // FUTURE: Calculate a weighted average of items
                }
            }
        }

        public TimeSpan? TimeSpan
        {
            get;
            set;
        }
    }
    public class TrackingTask : INotifyPropertyChanged
    {
        #region Description

        public string Description
        {
            get { return description; }
            set
            {
                if (description == value) return;
                description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
        private string description;

        #endregion

        #region DueDate

        public DueDate DueDate
        {
            get { return dueDate; }
            set
            {
                if (dueDate == value) return;
                dueDate = value;
                OnPropertyChanged(nameof(DueDate));
            }
        }
        private DueDate dueDate;

        #endregion

        public DateTime StartDate { get; set; }

        public EstimatedDuration Duration { get; set; }

        //public List<NotificationStrategy> NotificationStrategies{get;set;}

        public NotificationStrategy NotificationStrategy { get; set; }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

    }

#if UNUSED
    public static class TaskTests
    {
        public static void Test()
        {
            var task = new TrackingTask();

            task.Description = "Take out the garbage";
            task.Duration = new EstimatedDuration { TimeSpan = TimeSpan.FromMinutes(10) };
            task.DueDate = new DueDate { Date = new DateTime(2015, 05, 15, 16, 10, 15) };
            task.NotificationStrategy = NotificationStrategy.DefaultCriticalMustAck;

        }
    }
#endif

}

#endif