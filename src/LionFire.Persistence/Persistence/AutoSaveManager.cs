﻿#define TRACE_AUTOSAVE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Reactive;
using System.ComponentModel;
using System.Collections.Concurrent;
using LionFire.Structures;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Specialized;
using LionFire.Data.Async.Gets;
using LionFire.Results;
using LionFire.Data.Async.Sets;

namespace LionFire.Persistence
{
    public class AutoSaveOptions
    {
        public int AutoSaveThrottleMilliseconds { get; set; } = 3000;

        public bool AutoCreate { get; set; }
    }

    public class AutoSaveManager
    {
        public static AutoSaveManager Instance => Singleton<AutoSaveManager>.Instance;

        public AutoSaveOptions Options { get; set; } = new AutoSaveOptions();

        internal ConcurrentDictionary<ISetter, ThrottledChangeHandler> handlers = new ConcurrentDictionary<ISetter, ThrottledChangeHandler>();
    }

    public static class AutoSaveManagerExtensions
    {
        public static void QueueAutoSave(this ISetter saveable)
        {
            Trace.WriteLine($"Queued autosave for {saveable.GetType().Name}: {saveable}");
            var handler = GetHandler(saveable);
            handler.Queue();
        }

        private static ThrottledChangeHandler GetHandler(ISetter asset, Func<object, ITask<ITransferResult>> saveAction = null)
        {
            if (asset == null) return null;
            if (saveAction == null) saveAction = o => ((ISetter)o).Set().AsITask();
            return AutoSaveManager.Instance.handlers.GetOrAdd(asset, a =>
            {
                var wrappedChanged = a as INotifyWrappedValueChanged;
                if (wrappedChanged == null) { throw new ArgumentException("AutoSaved object must implement INotifyWrappedValueChanged."); }

                //var inpc = a as INotifyPropertyChanged ?? ((a as IReadWrapper<object>)?.Value as INotifyPropertyChanged);
                //if (inpc == null) { throw new ArgumentException("asset must implement INotifyPropertyChanged, or wrap an object via IWrapper that does."); }
                return new ThrottledChangeHandler(wrappedChanged, saveAction, TimeSpan.FromMilliseconds(AutoSaveManager.Instance.Options.AutoSaveThrottleMilliseconds));
            }
            );
        }

        private static void OnChangeQueueHandler(object sender)
        {
            var h = GetHandler(sender as ISetter);
            h?.Queue();
        }

        /// <summary>
        /// If the asset implements IChanged, only it will be used.  Otherwise, it tries both INotifyPropertyChanged on the asset, and also INotifyCollectionChanged on the asset's properties.
        /// </summary>
        /// <param name="saveable"></param>
        /// <param name="enable"></param>
        public static void EnableAutoSave(this ISetter saveable, bool enable = true, Func<object, ITask<ITransferResult>>? saveAction = null)
        {

            //bool attachedToSomething = false;
            var handler = GetHandler(saveable, saveAction ?? ((_) => saveable.Set().AsITask()));

            //var ic = saveable as IChanged;
            //if (ic != null) // TO C#7
            //{
            //    if (enable)
            //    {
            //        ic.Changed += OnChangeQueueHandler;
            //    }
            //    else
            //    {
            //        ic.Changed -= OnChangeQueueHandler;
            //    }
            //    attachedToSomething = true;
            //    return; // If this one is available, skip other options.
            //}

            //var inpc = saveable as INotifyPropertyChanged;
            //if (inpc == null)
            //{
            //    inpc = (saveable as IReadWrapper<object>)?.Value as INotifyPropertyChanged;
            //}
            //if (inpc != null)
            //{
            //    if (enable)
            //    {
            //        // Already attached in GetHandler
            //        attachedToSomething = true;
            //    }
            //    else
            //    {
            //        ThrottledChangeHandler removedHandler;
            //        if (AutoSaveManager.Instance.handlers.TryRemove(saveable, out removedHandler))
            //        {
            //            removedHandler.Dispose();
            //        }
            //    }
            //}

            //foreach (var pi in saveable.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            //{
            //    if (typeof(INotifyCollectionChanged).IsAssignableFrom(pi.PropertyType))
            //    {
            //        var incc = pi.GetValue(saveable) as INotifyCollectionChanged;
            //        incc.CollectionChanged += (s, e) => handler.Queue();
            //        attachedToSomething = true;
            //    }
            //}

            //if (!attachedToSomething)
            //{
            //    throw new ArgumentException("Does not implement INotifyPropertyChanged, and no other change interfaces are currently supported.", nameof(saveable));
            //}

        }

        private static void Incc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }


}
