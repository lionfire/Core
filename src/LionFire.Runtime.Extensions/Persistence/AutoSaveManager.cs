#define TRACE_AUTOSAVE
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

namespace LionFire.Persistence
{
    public class AutoSaveManager
    {
        public static AutoSaveManager Instance { get { return Singleton<AutoSaveManager>.Instance; } }

        internal ConcurrentDictionary<ISaveable, ThrottledChangeHandler<ISaveable>> handlers = new ConcurrentDictionary<ISaveable, ThrottledChangeHandler<ISaveable>>();
    }

    public static class AutoSaveManagerExtensions
    {
        public static readonly int AutoSaveThrottleMilliseconds = 2000;
        public static void QueueAutoSave(this ISaveable saveable)
        {
            Debug.WriteLine($"Queued autosave for {saveable.GetType().Name}");
            var handler = GetHandler(saveable);
            handler.Queue();
        }

        private static ThrottledChangeHandler<ISaveable> GetHandler(ISaveable asset)
        {
            if (asset == null) return null;
            return AutoSaveManager.Instance.handlers.GetOrAdd(asset, a => new ThrottledChangeHandler<ISaveable>((INotifyPropertyChanged)a,
                o => o.Save(), TimeSpan.FromMilliseconds(2000)));
        }

        private static void OnChangeQueueHandler(object sender)
        {
            var h = GetHandler(sender as ISaveable);
            h?.Queue();
        }

        /// <summary>
        /// If the asset implements IChanged, only it will be used.  Otherwise, it tries both INotifyPropertyChanged on the asset, and also INotifyCollectionChanged on the asset's properties.
        /// </summary>
        /// <param name="saveable"></param>
        /// <param name="enable"></param>
        public static void EnableAutoSave(this ISaveable saveable, bool enable = true)
        {

            bool attachedToSomething = false;
            var handler = GetHandler(saveable);

            var ic = saveable as IChanged;
            if (ic != null) // TO C#7
            {
                if (enable)
                {
                    ic.Changed += OnChangeQueueHandler;
                }
                else
                {
                    ic.Changed -= OnChangeQueueHandler; 
                }
                attachedToSomething = true;
                return; // If this one is available, skip other options.
            }

            var inpc = saveable as INotifyPropertyChanged;
            
            if (inpc != null)
            {
                if (enable)
                {
                    attachedToSomething = true;
                }
                else
                {
                    ThrottledChangeHandler<ISaveable> removedHandler;
                    if (AutoSaveManager.Instance.handlers.TryRemove(saveable, out removedHandler))
                    {
                        removedHandler.Dispose();
                    }
                }
            }

            foreach (var pi in saveable.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (typeof(INotifyCollectionChanged).IsAssignableFrom(pi.PropertyType))
                {
                    var incc = pi.GetValue(saveable) as INotifyCollectionChanged;
                    incc.CollectionChanged += (s, e) => handler.Queue();
                    attachedToSomething = true;
                }
            }

            if (!attachedToSomething)
            {
                throw new ArgumentException("Does not implement INotifyPropertyChanged, and no other change interfaces are currently supported.", nameof(saveable));
            }

        }

        private static void Incc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }


}
