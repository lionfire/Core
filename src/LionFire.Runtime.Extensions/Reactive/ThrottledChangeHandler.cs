using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reflection;
using System.Reactive.Subjects;


namespace LionFire.Reactive
{

    public class ThrottledChangeHandler<T> : IDisposable
    {
        INotifyPropertyChanged inpc;
        IDisposable subscription;
        IDisposable subscription2;
        Subject<int> thr;

        public ThrottledChangeHandler(INotifyPropertyChanged inpc, Action<T> action, TimeSpan delay)
        {
            if (inpc == null) throw new ArgumentNullException(nameof(inpc));
            this.inpc = inpc;
            if (!typeof(T).GetTypeInfo(). IsAssignableFrom(inpc.GetType().GetTypeInfo()))
            {
                throw new ArgumentException();
            }

            var obs = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(h => inpc.PropertyChanged += h, h2 => inpc.PropertyChanged -= h2);
            thr = new Subject<int>();
            subscription = obs.Throttle(delay).Subscribe(eventPattern => action((T)eventPattern.Sender));
            subscription2 = thr.Throttle(delay).Subscribe(_ => action((T)inpc));
        }

        public void Queue()
        {
            thr.OnNext(0);
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
