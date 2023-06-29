using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using LionFire.Structures;
using LionFire.Data.Gets;

namespace LionFire.Persistence
{

#if TODO // MOVE to LionFire.Refreencing.Application.dll ?
    public static class ThrottledSaveManagerExtensions
    {
        public IAppHost AddThrottledSaveManager(this IAppHost hos)
        {
            var app = LionFire.Applications.LionFireApp.Current;
            if (app != null)
            {
                app.Closing += app_Closing;
            }
        }
        void app_Closing(CancelableEventArgs obj)
        {
            try
            {
                LionFire.Applications.LionFireApp.Current.Closing -= app_Closing;
            }
            catch { }

            if (queue.Count > 0)
            {
                l.Debug("ThrottledSaveManager: queue still has " + queue.Count + " items.  Flushing now.");
                Flush();
            }
        }
    }        
#endif
    
    // See also .NET Core: LionFire.Assets.AutoSaveManager
    // TODO: Make generic version for throttling, with a configurable delegate
    public class ThrottledSaveManager
    {
        #region (Static)

        public static ThrottledSaveManager Instance { get { return Singleton<ThrottledSaveManager>.Instance; } }

        #endregion

        private System.Timers.Timer timer = new System.Timers.Timer();

        #region Construction

        public ThrottledSaveManager()
        {
            timer.Interval = 2000;
            timer.Elapsed += timer_Elapsed;

            throw new NotImplementedException("TODO: move ThrottledSaveManagerExtensions.AddThrottledSaveManager somewhere and make sure it gets called.");
        }


        #endregion

        #region Timer Event

        private readonly object saveLock = new object();

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Flush();
        }

        private void Flush()
        {
            lock (saveLock)
            {
                HashSet<ISets> saved = new HashSet<ISets>();

                if (queue.Count > 0)
                {
                    while (queue.TryDequeue(out ISets item))
                    {
                        if (saved.Contains(item))
                        {
                            continue;
                        }

                        if (item is ISets h)
                        {
                            try
                            {
                                h.Set();
                                l.Trace("UNTESTED - Delay saved " + item.ToString());
                            }
                            catch (Exception ex)
                            {
                                l.Error("[SAVE] Delayed save of '" + item.ToStringSafe() + "' failed: " + ex.ToString());
                            }
                        }
                        else
                        {
                            l.Warn("ThrottledSaveManager: item does not implement ISaveable: " + item.ToStringSafe());
                        }
                        saved.Add(item);
                    }
                }
                else // One extra tick, 
                {
                    timer.Enabled = false;
                }
            }
        }

        #endregion

        #region Manual Change Notifications

        private ConcurrentQueue<ISets> queue = new ConcurrentQueue<ISets>();

        public void OnChanged(ISets obj)
        {
            queue.Enqueue(obj);
            timer.Enabled = true;
        }

        #endregion

        #region Misc

        private static readonly ILogger l = Log.Get();

        #endregion
    }

}
