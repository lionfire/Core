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

namespace LionFire.Assets
{
    public class AutoSaveManager
    {
        public static AutoSaveManager Instance { get { return Singleton<AutoSaveManager>.Instance; } }

        internal ConcurrentDictionary<IAsset, ThrottledChangeHandler<IAsset>> handlers = new ConcurrentDictionary<IAsset, ThrottledChangeHandler<IAsset>>();

    }

    public static class AutoSaveManagerExtensions
    {
        public static readonly int AutoSaveThrottleMilliseconds = 2000;
        public static void QueueAutoSave(this IAsset asset)
        {
            var handler = GetHandler(asset);
            handler.Queue();
        }

        private static ThrottledChangeHandler<IAsset> GetHandler(IAsset asset)
        {
            if (asset == null) return null;
            return AutoSaveManager.Instance.handlers.GetOrAdd(asset, a => new ThrottledChangeHandler<IAsset>((INotifyPropertyChanged)a,
                o => IAssetExtensions.Save(o), TimeSpan.FromMilliseconds(2000)));
        }

        private static void OnChangeQueueHandler(object sender)
        {
            var h = GetHandler(sender as IAsset);
            h?.Queue();
        }

        /// <summary>
        /// If the asset implements IChanged, only it will be used.  Otherwise, it tries both INotifyPropertyChanged on the asset, and also INotifyCollectionChanged on the asset's properties.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="enable"></param>
        public static void EnableAutoSave(this IAsset asset, bool enable = true)
        {

            bool attachedToSomething = false;
            var handler = GetHandler(asset);

            var ic = asset as IChanged;
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

            var inpc = asset as INotifyPropertyChanged;
            
            if (inpc != null)
            {
                if (enable)
                {
                    attachedToSomething = true;
                }
                else
                {
                    ThrottledChangeHandler<IAsset> removedHandler;
                    if (AutoSaveManager.Instance.handlers.TryRemove(asset, out removedHandler))
                    {
                        removedHandler.Dispose();
                    }
                }
            }

            foreach (var pi in asset.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (typeof(INotifyCollectionChanged).IsAssignableFrom(pi.PropertyType))
                {
                    var incc = pi.GetValue(asset) as INotifyCollectionChanged;
                    incc.CollectionChanged += (s, e) => handler.Queue();
                    attachedToSomething = true;
                }
            }

            if (!attachedToSomething)
            {
                throw new ArgumentException("Does not implement INotifyPropertyChanged, and no other change interfaces are currently supported.", nameof(asset));
            }

        }

        private static void Incc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }


}
