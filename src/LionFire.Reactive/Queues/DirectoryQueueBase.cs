using LionFire.Execution;
using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using LionFire.Instantiating;

namespace LionFire.Queues
{

    public abstract class DirectoryQueueBase
    {
        #region Subdirectories

        public string InSubDir = "in";
        public string OutSubDir = "out";
        public string HandlingSubDir = "handling";
        public string UnhandledSubDir = "unhandled";
        public string FaultedSubDir = "faulted";
        public string TrashSubDir = "trash";
        public string LogSubDir = "log";

        #endregion

        public string QueueDir { get; set; }

    }

    public class TDirectoryQueue
    {
        public int PollMillisecondsInterval { get; set; }
        public bool WatchForChanges { get; set; }
        
    }


    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Subdirectories:
    ///  - In
    ///  - Handling (Filesystem move used to atomically )
    ///  - Out (if enabled -- makes this queue a sort of pipeline)
    ///  - Unhandled (if enabled)
    ///  - Trash (if enabled)
    ///  </remarks>
    public class FsDirectoryQueueBase : DirectoryQueueBase, IQueue, IStartable, IStoppable
    {

        public TDirectoryQueue Settings { get; set; } = new TDirectoryQueue
        {
            PollMillisecondsInterval = 1000,
        };

        #region Directories

        public string InDir => Path.Combine(QueueDir, InSubDir);
        public string OutDir => Path.Combine(QueueDir, OutSubDir);
        public string HandlingDir { get; set; }
        public string UnhandledDir => Path.Combine(QueueDir, UnhandledSubDir);
        public string FaultedDir => Path.Combine(QueueDir, FaultedSubDir);
        public string TrashDir => Path.Combine(QueueDir, TrashSubDir);
        public string LogDir => Path.Combine(QueueDir, LogSubDir);

        #endregion

        private Guid Guid = Guid.NewGuid();

        public void RestorePartiallyHandledMessages()
        {
            // for all handlers where proc id is gone, move handling messages back to in.

        }

        public void InitHandlerDir()
        {
            HandlingDir = Path.Combine(QueueDir, HandlingSubDir, Process.GetCurrentProcess().Id, this.Guid);
            //;
            //int i = 0;
            //do
            //{
            //    string suffix = i == 0 ? "" : "-" + i.ToString();
            //    string dir = Path.Combine(QueueDir, HandlingSubDir + suffix);
            //} while (Directory.Exists());
        }


        public void Handling(string key)
        {
            // REVIEW - extensions.  Could be "abcdef.tar.bz"
            var files = Directory.GetFiles(QueueDir, key + ".*", SearchOption.TopDirectoryOnly);

            if (files.Length == 0)
            {
                // Assume another handler handled it
                return;
            }
            else if (files.Length > 1)
            {
                throw new Exception("TODO: Register error state in queue -- multiple identical keys with different file extensions");
            }
            else // files.Length == 1
            {
                File.Move(files[0], Path.Combine(HandlingDir, Path.GetFileName(files[0])));
            }
        }

        Task pollTask;
        private void StartPolling()
        {
            isRunning = true;
            pollTask = PollLoop();
        }
        private void StopPolling()
        {
            isRunning = false;
        }

        public bool isRunning;
        public async Task PollLoop()
        {
            string KeyForPath(string path) => Path.GetFileNameWithoutExtension(path);

            while (isRunning)
            {
                await Task.Delay(1000);

                bool addedSomething = false;
                foreach (var key in Directory.GetFiles(QueueDir).Select(f => KeyForPath(f)).Where(f => !objects.Keys.Contains(f)))
                {
                }

                foreach (var key in Directory.GetFiles(QueueDir).Select(f => KeyForPath(f)).Where(f => !objects.Keys.Contains(f)))
                {
                    addedSomething = true;
                    objects.TryAdd(key, null);
                }

                // TODO: detect removals

                if (addedSomething)
                {
                    HasNewItem?.Invoke(this);
                }
            }
        }

        public Task Start()
        {
            throw new NotImplementedException();
        }

        public void Peek(Func<IQueue, QueueMessageEnvelope, bool> handler)
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe(IObserver<QueueMessageEnvelope> observer)
        {
            throw new NotImplementedException();
        }

        public Task Stop(StopMode mode = StopMode.GracefulShutdown, StopOptions options = StopOptions.StopChildren)
        {
            throw new NotImplementedException();
        }

        ConcurrentDictionary<string, object> objects = new ConcurrentDictionary<string, object>();

        public event Action<IQueue> HasNewItem;
        public event Action<QueueMessageEnvelope> Received;
        public event Action<IQueue, string, object> ItemRemoved;

        public IEnumerable<object> Items
        {
            get
            {
                return objects.Values;
            }
        }

        public QueueFlags Flags { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int UnhandledMessages { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
