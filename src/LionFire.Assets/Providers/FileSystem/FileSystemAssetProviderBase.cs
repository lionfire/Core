using System.Collections.Generic;
using System.Linq;
using System.IO;
using LionFire.Instantiating;
using LionFire.Persistence;
using LionFire.MultiTyping;
using System.Threading.Tasks;
using System;

namespace LionFire.Assets.Providers.FileSystem
{
    // FUTURE: Integrate with Serializer framework to try loading files based on extension


    public abstract class FileSystemAssetProviderBase : INotifyPersistence
    {
        public string RootDir { get; set; }

        #region Construction and Initialization

        public FileSystemAssetProviderBase(string rootDir)
        {
            RootDir = rootDir;
        }


        protected void InitRootDir()
        {
            if (RootDir == null)
            {
                RootDir = LionFireEnvironment.Directories.AppProgramDataDir;
            }

            if (!Directory.Exists(RootDir))
            {
                Directory.CreateDirectory(RootDir);
            }
        }

        #endregion

        public void CreateDirIfNeeded(object obj, string assetSubpath = null, object context = null)
        {
            var dir = GetDir(obj, assetSubpath, context);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public string GetDir(object obj, string assetSubpath = null, object context = null)
        {
            var pc = context.ObjectAsType<PersistenceContext>();
            return Path.GetDirectoryName(Path.Combine(pc?.RootPath ?? RootDir, AssetPathUtils.GetSubpath(obj, assetSubpath, context)));
        }

        public string GetPath<T>(string assetSubpath = null, object context = null)
        {
            var pc = context.ObjectAsType<PersistenceContext>();
            return Path.Combine(pc?.RootPath ?? RootDir, AssetPathUtils.GetSubpath<T>(assetSubpath, context)) + (assetSubpath == null ? "" : FileExtensionWithDot); ;
        }
        public string GetPath(object obj, string assetSubpath = null, object context = null)
        {
            var pc = context.ObjectAsType<PersistenceContext>();
            return Path.Combine(pc?.RootPath ?? RootDir, AssetPathUtils.GetSubpath(obj, assetSubpath, context)) + (assetSubpath == null ? "" : FileExtensionWithDot); ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Changes won't be reflected in file system watcher until EffectiveListeningToPersistenceEvents is toggled.
        /// </remarks>
        public abstract string FileExtension { get; }
        public string FileExtensionWithDot { get { return (string.IsNullOrWhiteSpace(FileExtension) ? "" : "." + FileExtension); } }


        public async Task<IEnumerable<string>> Find<T>(string searchString = null, object context = null)
        {
            return await Task.Run(() =>
            {
                var dir = GetPath<T>(context: context);
                if (searchString == null) searchString = "*";
                var result = new List<string>();
                foreach (var path in Directory.GetFiles(dir, searchString + FileExtensionWithDot))
                {
                    var assetName = path.Replace(dir, "").TrimStart('/').TrimStart('\\').Replace(FileExtensionWithDot, "");
                    result.Add(assetName);
                }
                return (IEnumerable<string>)result;
            });
        }

        #region INotifyPersistenceEvent

        public virtual bool ListenToPersistenceEventsRecursively => true;
        protected virtual bool DiscardDisabledFSW => true;

        public event PersistenceEventHandler PersistenceEvent
        {
            add
            {
                persistenceEvent += value;
                EvaluateEffectiveListeningToPersistenceEvents();
            }
            remove
            {
                persistenceEvent -= value;
                EvaluateEffectiveListeningToPersistenceEvents();
            }
        }
        private event PersistenceEventHandler persistenceEvent;

        protected NotifyFilters NotifyFilters { get; set; } = NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.LastWrite;

        #region EffectiveListeningToPersistenceEvents

        FileSystemWatcher fsw;

        public bool EffectiveListeningToPersistenceEvents
        {
            get
            {
                return fsw != null && fsw.EnableRaisingEvents;
            }
            set
            {
                if (value == EffectiveListeningToPersistenceEvents) return;

                if (value)
                {
                    if (fsw == null)
                    {
                        fsw = new FileSystemWatcher(RootDir);
                    }
                    if (!string.IsNullOrWhiteSpace(FileExtensionWithDot)) fsw.Filter = "*" + FileExtensionWithDot;
                    fsw.NotifyFilter = NotifyFilters;
                    fsw.Changed += Fsw_Changed;
                    fsw.Created += Fsw_Created;
                    fsw.Deleted += Fsw_Deleted;
                    fsw.Error += Fsw_Error;
                    fsw.Renamed += Fsw_Renamed;
                    fsw.EnableRaisingEvents = true;
                }
                else
                {
                    fsw.EnableRaisingEvents = false;
                    if (DiscardDisabledFSW)
                    {
                        fsw.Dispose();
                        fsw = null;
                    }
                }
            }
        }

        private void Fsw_Renamed(object sender, RenamedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Fsw_Error(object sender, ErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Fsw_Deleted(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Fsw_Created(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Fsw_Changed(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void EvaluateEffectiveListeningToPersistenceEvents()
        {
            if(ListeningToPersistenceEvents.HasValue) { EffectiveListeningToPersistenceEvents = ListeningToPersistenceEvents.Value; }
            if (persistenceEvent != null)
            {

                EffectiveListeningToPersistenceEvents = true;
            }
        }

        public void OnPersistenceEvent(PersistenceEvent ev) => throw new NotImplementedException();


        #endregion

        public bool? ListeningToPersistenceEvents
        {
            get
            {
                if (listeningToPersistenceEvents.HasValue)
                {
                    return listeningToPersistenceEvents.Value;
                }
                return EffectiveListeningToPersistenceEvents;
            }
            set
            {
                if (listeningToPersistenceEvents == value) return;
                listeningToPersistenceEvents = value;
                EvaluateEffectiveListeningToPersistenceEvents();
            }
        }
        private bool? listeningToPersistenceEvents;

        #endregion
    }
}
