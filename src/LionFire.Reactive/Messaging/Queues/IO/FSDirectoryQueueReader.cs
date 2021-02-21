using LionFire.Execution;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using LionFire.Validation;
using System.Reactive.Subjects;
using LionFire.DependencyInjection;
using LionFire.Serialization;
using System.Linq;
using LionFire.Threading;
using LionFire.Dependencies;
using System.Threading;

namespace LionFire.Messaging.Queues.IO
{
    public static class DirectoryExtensions
    {
        public static void CreateDirectoryIfMissing(this string path)
        {
            if (Directory.Exists(path)) return;
            Directory.CreateDirectory(path);
        }
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
    public class FSDirectoryQueueReader : DirectoryQueueReader<TDirectoryQueueReader>, IQueueReader, IStartable, IStoppable, IInitializable
    {

        #region Directories

        public string QueueDir { get; set; }

        public string HandlingDir { get; set; }

        #region Directories (Derived)

        public string InDir => Path.Combine(QueueDir, DirectoryQueue.InSubDir);
        public string OutDir => Path.Combine(QueueDir, DirectoryQueue.OutSubDir);
        public string UnhandledDir => Path.Combine(QueueDir, DirectoryQueue.UnhandledSubDir);
        public string FaultedDir => Path.Combine(QueueDir, DirectoryQueue.FaultedSubDir);
        public string TrashDir => Path.Combine(QueueDir, DirectoryQueue.TrashSubDir);
        public string LogDir => Path.Combine(QueueDir, DirectoryQueue.LogSubDir);

        #endregion

        #endregion

        #region Relationships

        //[Dependency]
        ISerializationService SerializationService
        {
            get
            {
                if (serializationService == null)
                {
                    serializationService = DependencyContext.Current.GetService<ISerializationService>();
                }
                return serializationService;
            }
        }
        ISerializationService serializationService;

        #endregion

        #region Initialization

        private bool isInitialized;

        public Task<bool> Initialize()
        {
            if (isInitialized) return Task.FromResult(true);
            ValidationContext vc = default(ValidationContext);
            if (QueueDir == null)
            {
                this.QueueDir = ResolveUriToDiskDir(Template.QueueDataUri, ref vc); // TODO FIXME vc code flow
            }

            InitHandlerDir();
            InDir.CreateDirectoryIfMissing();

            if (AutoCleanDeadQueues)
            {
                CleanDeadQueues();
            }

            if (vc.Valid) isInitialized = true;
            return Task.FromResult(isInitialized);
        }

        private void CleanDeadQueues()
        {
            // TODO - delete queue handler directories for processes that aren't running.  
            // FUTURE: If this process, delete if GUID isn't registered as an active queue
        }

        protected string ResolveUriToDiskDir(string uri, ref ValidationContext validationContext)
        {
            if (uri.StartsWith("file:")) throw new NotImplementedException("TODO");
            return uri;
        }

        public void InitHandlerDir()
        {
            HandlingDir = Path.Combine(QueueDir, DirectoryQueue.HandlingSubDir, System.Diagnostics.Process.GetCurrentProcess().Id.ToString(), this.Guid.ToString());
            HandlingDir.CreateDirectoryIfMissing();
            //;
            //int i = 0;
            //do
            //{
            //    string suffix = i == 0 ? "" : "-" + i.ToString();
            //    string dir = Path.Combine(QueueDir, HandlingSubDir + suffix);
            //} while (Directory.Exists());
        }

        #endregion

        #region IsPolling

        public bool IsPolling
        {
            get { return isPolling; }
            set
            {
                if (isPolling == value) return;
                isPolling = value;
                if (isPolling)
                {
                    pollTask = PollLoop();
                }
                else
                {
                    // should quit on its own as it watches isPolling
                }
            }
        }
        private bool isPolling;

        #endregion

        #region Handling

        private async Task TryHandleFile(string path)
        {
#if SanityChecks
            // path in HandlingDir
#endif
            try
            {
                if (IsAsync)
                {
                    await Task.Run(() =>
                    {
                        var obj = serializationService.Strategies.First().ToObject<object>(File.ReadAllText(path));
                        TryHandle(path, obj);
                    });
                }
                else
                {
                    var obj = serializationService.Strategies.First().ToObject<object>(File.ReadAllText(path));
                    TryHandle(path, obj);
                }
            }
            catch (Exception ex)
            {
                OnHandleException(path, ex);
            }
        }

        private void OnHandleException(string path, Exception ex)
        {
            File.Move(path, Path.Combine(FaultedDir, Path.GetFileName(path)));
        }

        private void OnHandlingFinished(string path, bool handled)
        {
            if (handled)
            {
                if (QueueFlags.HasFlag(QueueReaderFlags.ExplicitDelete))
                {
                    File.Move(path, Path.Combine(HandlingDir, Path.GetFileName(path)));
                }
                else
                {
                    File.Delete(path);
                }
            }
            else
            {
                if (!QueueFlags.HasFlag(QueueReaderFlags.DiscardUnhandledMessages))
                {
                    UnhandledDir.CreateDirectoryIfMissing();
                    File.Move(path, Path.Combine(UnhandledDir, Path.GetFileName(path)));
                }
                else
                {
                    File.Delete(path);
                }
            }
        }

        public bool IsAsync { get; set; }
        protected virtual void TryHandle(string path, object obj)
        {
            var env = new MessageEnvelope()
            {
                Header = Path.GetFileNameWithoutExtension(path),
                Payload = obj,
            };
            try
            {
                RaiseMessageReceived(env);
                //messagesSubject.OnNext(env);
            }
            catch (Exception ex)
            {
                OnHandleException(path, ex);
            }

            OnHandlingFinished(path, env.IsHandled);
        }

        private string KeyForPath(string path) => Path.GetFileNameWithoutExtension(path);

        Task pollTask;
        public async Task PollLoop()
        {
            while (isPolling)
            {
                await Task.Delay(1000);

                List<string> handlingFiles = new List<string>();
                if (Directory.Exists(InDir))
                {
                    foreach (var file in Directory.GetFiles(InDir))
                    {
                        var newPath = Path.Combine(HandlingDir, Path.GetFileName(file));
                        try
                        {
                            // TODO: Try opening file for exclusive read/write to guarantee exclusive access???
                            File.Move(file, newPath);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(this.GetType().Name + " queue exception moving file -- it may still be being written/downloaded: " + ex.ToString());
                            continue;
                        }
                        handlingFiles.Add(newPath);
                    }
                }

                Task.Run(async () =>
                {
                    foreach (var newPath in handlingFiles)
                    {
                        if (IsAsync)
                        {
                            TryHandleFile(newPath).FireAndForget();
                        }
                        else
                        {
                            await TryHandleFile(newPath);
                        }
                    }
                }).FireAndForget();

                //IEnumerable<string> enumeration = Directory.GetFiles(InDir);
                //enumeration.Select(f => DoIt(f));

                //if (IsAsync)
                //{
                //    enumeration = enumeration.AsParallel();
                //}
                //foreach (var _ in enumeration) { }
                //foreach (var file in enumeration
                //    //.Select(f => KeyForPath(f)).Where(f => !objects.Keys.Contains(f))
                //    )
                //{
                //    if (IsAsync)
                //    {
                //        await Task.Run(

                //    }
                //    else
                //    {
                //        DoIt(file);
                //    }
                //}
            }

            //object DoIt(string file)
            //{
            //    try
            //    {
            //        var newPath = Path.Combine(HandlingDir, Path.GetFileName(file));
            //        File.Move(file, newPath);
            //        TryHandleFile(newPath);
            //    }
            //    catch (Exception ex)
            //    {
            //        Debug.WriteLine(this.GetType().Name + " queue exception: " + ex.ToString());
            //    }
            //    return null;
            //}
        }

        #endregion

        #region Start / Stop

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await Initialize();
            if (Template.PollMillisecondsInterval > 0)
            {
                IsPolling = true;
            }
        }

        public Task StopAsync(CancellationToken? cancellationToken = default)
        {
            IsPolling = false;
            return Task.CompletedTask;
        }

        

        #endregion

        #region IObservable<MessageEnvelope>

        //public IObservable<MessageEnvelope> Observable => messagesSubject;
        //private AsyncSubject<MessageEnvelope> messagesSubject = new AsyncSubject<MessageEnvelope>();



        #endregion

        #region (Public) Methods



        //public void Add(MessageEnvelope envelope)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

        //public void Peek(Func<IQueue, MessageEnvelope, bool> handler)
        //{
        //    IsPolling = false;
        //}
        #region Events

        #endregion

        //ConcurrentDictionary<string, object> objects = new ConcurrentDictionary<string, object>();

        //public event Action<IQueue> HasNewItem;

        //public event Action<IQueue, string, object> Handled;
        //public event Action<IQueue, string, object> Unhandled;
        //public event Action<IQueue, string, object, Exception> Faulted;
        //public event Action<MessageEnvelope> Added;

        //public int UnhandledMessages { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


    }
}
