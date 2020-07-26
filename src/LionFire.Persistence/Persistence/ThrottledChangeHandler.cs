using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using LionFire.Structures;
using LionFire.Results;
using System.Diagnostics;
#if Rx
using System.Reactive.Linq;
using System.Reactive.Subjects;
#endif
using System.Reflection;

namespace LionFire.Reactive
{
    public class ThrottledChangeHandler : IDisposable
    {
        public INotifyWrappedValueChanged Wrapper { get; }
        IDisposable throttledChangeEvent;
        IDisposable throttledManualQueue;

#if Rx
        Subject<int> manualTrigger;
#endif

        public ThrottledChangeHandler(INotifyWrappedValueChanged wrapper, Func<object, Task<ISuccessResult>> action, TimeSpan delay)
        {
            Wrapper = wrapper ?? throw new ArgumentNullException(nameof(wrapper));
#if Rx
            var obs = Observable.FromEvent<Action<INotifyWrappedValueChanged>, INotifyWrappedValueChanged>(h => Wrapper.WrappedValueChanged += h, h2 => Wrapper.WrappedValueChanged -= h2);
            manualTrigger = new Subject<int>();
            //Wrapper.WrappedValueChanged += Wrapper_WrappedValueChanged;
            //(Wrapper as INotifyWrappedValueReplaced).WrappedValueForFromTo += ThrottledChangeHandler_WrappedValueForFromTo;
            throttledChangeEvent = obs.Throttle(delay).Subscribe(w => action(w));
            throttledManualQueue = manualTrigger.Throttle(delay).Subscribe(_ => action(Wrapper));
#else
            throw new NotImplementedException();
#endif
        }

        //private void ThrottledChangeHandler_WrappedValueForFromTo(INotifyWrappedValueReplaced arg1, object arg2, object arg3)
            //=> Debug.WriteLine("ThrottledChangeHandler_WrappedValueForFromTo");
        //private void Wrapper_WrappedValueChanged(INotifyWrappedValueChanged obj) => Debug.WriteLine("Wrapper_WrappedValueChanged");

        public void Queue()
        {
#if Rx
            manualTrigger.OnNext(0);
#endif
        }

        public void Dispose()
        {
            throttledChangeEvent?.Dispose();
            throttledChangeEvent = null;
            throttledManualQueue?.Dispose();
            throttledManualQueue = null;
        }
    }
}