using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
#if Rx
using System.Reactive.Linq;
using System.Reactive.Subjects;
#endif
using System.Reflection;

namespace LionFire.Reactive
{
    public class ThrottledChangeHandler : IDisposable
    {
        INotifyPropertyChanged inpc;
        IDisposable subscription;
        IDisposable subscription2;
#if Rx
        Subject<int> thr;
#endif

        public ThrottledChangeHandler(INotifyPropertyChanged inpc, Action<object> action, TimeSpan delay)
        {
#if Rx
            if (inpc == null) throw new ArgumentNullException(nameof(inpc));
            this.inpc = inpc;
            //if (!typeof(T).GetTypeInfo().IsAssignableFrom(inpc.GetType().GetTypeInfo()))
            //{
            //    throw new ArgumentException();
            //}

            var obs = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(h => inpc.PropertyChanged += h, h2 => inpc.PropertyChanged -= h2);
            thr = new Subject<int>();
            subscription = obs.Throttle(delay).Subscribe(eventPattern => action(eventPattern.Sender));
            subscription2 = thr.Throttle(delay).Subscribe(_ => action(inpc));
#else
            throw new NotImplementedException();
#endif
        }

        public void Queue()
        {
#if Rx
            thr.OnNext(0);
#endif
        }

        public void Dispose()
        {
            subscription?.Dispose();
            subscription = null;
            subscription2?.Dispose();
            subscription2 = null;
        }
    }
}