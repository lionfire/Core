using LionFire.Persistence;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Persistence
{
    // See also .NET Core: LionFire.Assets.AutoSaveManager
    // TODO: Make generic version for throttling, with a configurable delegate
    public class ThrottledSaveManager
    {

        public static ThrottledSaveManager Instance { get { return Singleton<ThrottledSaveManager>.Instance; } }

        System.Timers.Timer timer = new System.Timers.Timer();

        #region Construction

        public ThrottledSaveManager()
        {
            var app = LionFire.Applications.LionFireApp.Current;
            if (app != null)
            {
                app.Closing += app_Closing;
            }

            timer.Interval = 2000;
            timer.Elapsed += timer_Elapsed;
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
                l.Debug("ThrottledSaveManager: queue still has " + queue.Count  + " items.  Flushing now.");
                Flush();
            }                        
        }

        #endregion

        #region Timer Event

        private object saveLock = new object();

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Flush();
        }

        void Flush()
        {
            lock (saveLock)
            {
                HashSet<ISaveable> saved = new HashSet<ISaveable>();

                if (queue.Count > 0)
                {
                    ISaveable item;
                    while (queue.TryDequeue(out item))
                    {
                        if (saved.Contains(item)) continue;

                        ISaveable h = item as ISaveable;
                        if (h != null)
                        {
                            try
                            {
                                h.Save();
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

        ConcurrentQueue<ISaveable> queue = new ConcurrentQueue<ISaveable>();

        public void OnChanged(ISaveable obj)
        {
            //l.Trace();
            queue.Enqueue(obj);
            timer.Enabled = true;
        }

        #endregion


        #region Misc

        private static ILogger l = Log.Get();

        #endregion
    }

}
