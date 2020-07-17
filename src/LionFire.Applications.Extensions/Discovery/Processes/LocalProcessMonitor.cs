using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using LionFire.Services;
using System.IO;
using LionFire.Collections;
using LionFire.Discovery;
using System.ComponentModel;
using System.Collections;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using Timer = System.Timers.Timer;

namespace LionFire.Processes
{
    public class LocalProcessMonitorSettings
    {
        #region Properties

        public bool UsePolling = true;

        public bool UseWatcher = true;

        public int PollTimer = 2000;

        #endregion
    }

    public class LocalProcessMonitor : DiscoveryServiceBase<RunFile, LocalProcessMonitorSettings>, IDisposable, INotifyPropertyChanged, IHostedService
    {
        private static ILogger l = Log.Get();

        private Timer Timer;
        private FileSystemWatcher fsw;

        public LocalProcessMonitor()
        {
            //runFilesReadOnly = new ReadOnlySynchronizedObservableCollection<RunFile>(runfiles);
            fsw = new FileSystemWatcher();
            Timer = new Timer
            {
                Interval = 2000,
                AutoReset = false
            };
            Timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
        }

        void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Update();
            }
            catch (Exception ex)
            {
                l.Error(ex.ToString());
            }
            finally
            {
                Timer.Enabled = true;
            }
        }

        DateTime lastUpdate = DateTime.MinValue;

        public void Update()
        {
            if ((DateTime.UtcNow - lastUpdate + TimeSpan.FromSeconds(1)) < TimeSpan.FromMilliseconds(Timer.Interval))
            {
                l.Trace("Skipping update due to last one occurring recently.");
                return;
            }

            var removals = new List<RunFile>(items);

            foreach (RunFile runFile in RunFileManager.GetInstances())
            {
                if (removals.Contains(runFile))
                {
                    removals.Remove(runFile);
                }
                UpdateRunFile(runFile);
            }

            foreach (var runFile in removals)
            {
                items.Remove(runFile);
            }
            lastUpdate = DateTime.UtcNow;
        }

        public bool isStarted =false;

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                isStarted = true;
                if (Settings.UsePolling)
                {
                    Timer.Interval = Settings.PollTimer;
                    Timer.Enabled = true;
                }
                if (Settings.UseWatcher)
                {
                    if (fsw != null)
                    {
                        fsw.Dispose();
                        fsw = new FileSystemWatcher();
                    }

                    fsw.Path = RunFileManager.RunFileDirectory;
                    fsw.Created += new FileSystemEventHandler(fsw_Created);
                    fsw.Deleted += new FileSystemEventHandler(fsw_Deleted);
                    fsw.EnableRaisingEvents = true;
                }
            });
        }

        void fsw_Deleted(object sender, FileSystemEventArgs e)
        {
            if (!isStarted) return;
            UpdateRunFile(RunFile.FromPath(e.FullPath), false);
        }

        void fsw_Created(object sender, FileSystemEventArgs e)
        {
            if (!isStarted) return;
            UpdateRunFile(RunFile.FromPath(e.FullPath));
        }

        public void UpdateRunFile(RunFile runFile, bool exists = true)
        {
            if (items.Contains(runFile))
            {
                if (!exists)
                {
                    items.Remove(runFile);
                }
            }
            else
            {
                if (exists)
                {
                    items.Add(runFile);
                }
            }
        }

        //protected override void OnContinue()
        //{
        //    base.OnContinue();
        //    if (Settings.UsePolling)
        //    {
        //        Timer.Interval = Settings.PollTimer;
        //        Timer.Enabled = true;
        //    }
        //    if (Settings.UseWatcher)
        //    {
        //        fsw.EnableRaisingEvents = true;
        //    }
        //}

        //protected override void OnPause()
        //{
        //    base.OnPause();
        //    Timer.Enabled = false;
        //    fsw.EnableRaisingEvents = false;
        //}

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            isStarted = false;
            Timer.Enabled = false;
            fsw.EnableRaisingEvents = false;
            return Task.CompletedTask;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
        }

        public void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                if (fsw != null)
                {
                    fsw.Dispose();
                    fsw = null;
                }
                if (Timer != null)
                {
                    Timer.Dispose();
                    Timer = null;
                }
            }
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var ev = PropertyChanged;
            if (ev != null) ev(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}

