#if TODO
#define STATS

using LionFire.Applications;
using LionFire.ObjectBus;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire
{

    public class Timing : IDisposable
    {
        #region Fields

        public string Name;

        public DateTime Start;
        public DateTime Stop;
        public int Count = int.MinValue;

        #region Derived

        public TimeSpan Elapsed { get { if (Start == default(DateTime) || Stop == default(DateTime))return default(TimeSpan); return Stop - Start; } }

        #endregion

        #endregion

        #region (Static) Factory methods

        public static Timing StartNow(string name)
        {
#if STATS
            return new Timing(name, DateTime.UtcNow);
#else
            return null;
#endif

        }
        public static void RecordFromStart(string name)
        {
#if STATS
            using (new Timing(name)) { }
#endif
        }

        #endregion

        #region Construction

        public Timing(string name = null)
        {
            this.Name = name;
        }
        public Timing(string name, DateTime start)
            : this(name)
        {
            Start = start;
        }

        #endregion

        #region (Public) Stop Methods

        [Conditional("STATS")]
        public void StopAndRecord()
        {
            if (Start == default(DateTime)) Start = LionFireApp.Current.StartTime;

            Stop = DateTime.UtcNow;
            TimingManager.Instance.Record(this);
        }
        [Conditional("STATS")]
        public void StopAndLog()
        {
            if (Start == default(DateTime)) Start = LionFireApp.Current.StartTime;

            Stop = DateTime.UtcNow;
            l.Debug(this.ToString());
            //TimingManager.Instance.Record(this);
        }

        #endregion

        #region Dispose

        public void Dispose() // For using
        {
            StopAndRecord();
        }

        #endregion

        #region Misc

        public override string ToString()
        {
            return "[TIMING '"+Name+"': " +Elapsed.TotalMilliseconds+ "ms]";
            return base.ToString();
        }
        private static ILogger l
        {
            get
            {
                if(_l==null)
                { 
                _l = Log.Get();
            }
                return _l;
            }
        }
        private static ILogger _l = null;

        #endregion

    }

    public class TimingManager
    {
        #region (Static) Singleton

        public static TimingManager Instance { get { return Singleton<TimingManager>.Instance; } }

        #endregion

        #region Record

        public void Record(Timing timing)
        {
#if STATS
            queue.Enqueue(timing);
            ThreadPool.QueueUserWorkItem(x =>
                {
                    Flush();
                });
#endif
        }

        #endregion

        ConcurrentQueue<Timing> queue 
            #if STATS
            = new ConcurrentQueue<Timing>()
#endif
            ;

        public TimingManager()
        {
            #if STATS
            string dateTimeFormat = "yyyy.MM.dd HH:mm:ss";

            {
                var column = new TimingColumn("Date", x => DateTime.UtcNow.ToString("yyyy.MM.dd HH:mm:ss"));
                columns.Add(column.Name, column);
            }
            {
                var column = new TimingColumn("V1", x => LionEnvironment.VersionStringComponents[0]);
                columns.Add(column.Name, column);
            }
            {
                var column = new TimingColumn("V2", x => LionEnvironment.VersionStringComponents[1]);
                columns.Add(column.Name, column);
            }
            {
                var column = new TimingColumn("V3", x => LionEnvironment.VersionStringComponents[2]);
                columns.Add(column.Name, column);
            }
            {
                var column = new TimingColumn("V4", x => LionEnvironment.VersionStringComponents[3]);
                columns.Add(column.Name, column);
            }
            {
                var column = new TimingColumn("DEV", x => LionFireApp.IsDevMode ? "DEV" : "");
                columns.Add(column.Name, column);
            }
            {
                var column = new TimingColumn("DEBUG", x => LionFireApp.IsDebug ? "DEBUG" : "");
                columns.Add(column.Name, column);
            }
            {
                var column = new TimingColumn("TRACE", x => LionFireApp.IsTrace ? "TRACE" : "");
                columns.Add(column.Name, column);
            }
            {
                var column = new TimingColumn("Debugger", x => System.Diagnostics.Debugger.IsAttached ? "DEBUGGING" : "");
                columns.Add(column.Name, column);
            }
            {
                var column = new TimingColumn("Build", x => LionEnvironment.BuildType);
                columns.Add(column.Name, column);
            }

            {
                var column = new TimingColumn("Name", x => x.Name);
                columns.Add(column.Name, column);
            }

            {
                var column = new TimingColumn("Start", x => x.Start == default(DateTime) ? "" : x.Start.ToString(dateTimeFormat));
                columns.Add(column.Name, column);
            }
            {
                var column = new TimingColumn("Stop", x => x.Stop == default(DateTime) ? "" : x.Stop.ToString(dateTimeFormat));
                columns.Add(column.Name, column);
            }
            {
                var column = new TimingColumn("Elapsed", x => x.Elapsed == default(TimeSpan) ? "" : x.Elapsed.TotalMilliseconds.ToString());
                columns.Add(column.Name, column);
            }
            {
                var column = new TimingColumn("Count", x => x.Count == int.MinValue ? "" : x.Count.ToString());
                columns.Add(column.Name, column);
            }
#endif
        }

        private Dictionary<string, TimingColumn> columns
#if STATS
            = new Dictionary<string, TimingColumn>()
#endif
            ;

        public TimeSpan TimeFromStart
        {
            get
            {
                return DateTime.UtcNow - LionFireApp.Current.StartTime;
            }
        }

        [Conditional("STATS")]
        public void Flush()
        {
            try
            {
                if (VosApp.Instance == null)
                {
                    l.Trace("VosApp not initialized yet");
                }
                CreateIfNeeded();
                lock (writeLock)
                {
                    using (var tw = new StreamWriter(new FileStream(TimingsPath, FileMode.Append, FileAccess.Write, FileShare.Read)))
                    {
                        Timing timing;
                        while (queue.TryDequeue(out timing))
                        {
                            bool isFirst = true;
                            foreach (var col in columns.Values)
                            {
                                if (!isFirst) { tw.Write(","); } else { isFirst = false; }
                                tw.Write(col.GetValue(timing));
                            }
                            tw.WriteLine();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                l.Warn("Exception when flushing timings: " + ex.ToString());
            }
        }
        private object writeLock = new object();

        private void CreateIfNeeded()
        {
            if (!Directory.Exists(StatsDir)) Directory.CreateDirectory(StatsDir);
            lock (writeLock)
            {
                if (!File.Exists(TimingsPath))
                {
                    using (var tw = new StreamWriter(new FileStream(TimingsPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Delete)))
                    {
                        bool isFirst = true;
                        foreach (var col in columns.Values)
                        {
                            if (!isFirst) { tw.Write(","); } else { isFirst = false; }
                            tw.Write(col.Name);
                        }
                        tw.WriteLine();
                    }
                }
            }
        }

        public string StatsDir
        {
            get {
                return Path.Combine(LionEnvironment.CommonApplicationFolderPath, "Stats"); }
        }
        public string TimingsPath
        {
            get { return Path.Combine(StatsDir, "timings.csv"); }
        }

        private static ILogger l = Log.Get();

    }
    public class TimingColumn
    {
        public TimingColumn(string name, Func<Timing, string> provider) { this.Name = name; this.ValueProvider = provider; }
        public string Name;
        public string GetValue(Timing o) { return ValueProvider == null ? "" : ValueProvider(o); }
        public Func<Timing, string> ValueProvider;
    }
}
#endif