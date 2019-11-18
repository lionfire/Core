using Caliburn.Micro;
using LionFire.Applications;
using LionFire.Dependencies;
using LionFire.Structures;
using LionFire.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire
{
    public interface IEventMessage { }
    public static class EventExtensions
    {
        // ENH FUTURE: pass through all the Caliburn Micro async publish options: current thread, background thread, UI thread, etc.

        public static Task Publish(this IEventMessage msg)
        {
            //LionFireApp.Instance.EventAggregator.Publish(msg); // LEGACY
            return DependencyContext.Current.GetService<IEventAggregator>().PublishOnBackgroundThreadAsync(msg);
        }

        public static Task Publish(this Type messageType) // TOPROFILE
        {
            var tSing = typeof(ManualSingleton<>).MakeGenericType(messageType);
            throw new Exception("TOTEST: nameof(ManualSingleton<object>.GuaranteedInstance) is \"GuaranteedInstance\": " + nameof(ManualSingleton<object>.GuaranteedInstance));
            var mi = tSing.GetProperty(nameof(ManualSingleton<object>.GuaranteedInstance), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            var obj = mi.GetValue(null);
            //LionFireApp.Instance.EventAggregator.Publish(obj); // LEGACY
            return DependencyContext.Current.GetService<IEventAggregator>().PublishOnBackgroundThreadAsync(obj);
        }
    }
}
