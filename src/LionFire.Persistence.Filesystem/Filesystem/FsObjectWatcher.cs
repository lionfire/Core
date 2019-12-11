using LionFire.ObjectBus.Filesystem;
using LionFire.Persistence.Filesystem;
using LionFire.Referencing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus.Filesystem
{
    
    public class FsObjectWatcher : IObjectWatcher
    {
        #region FileSystemWatcher

        public FileSystemWatcher FSW
        {
            get { return fsw; }
            set
            {
                if (fsw != null)
                {
                    fsw.Dispose();
                }
                fsw = value;
            }
        }
        private FileSystemWatcher fsw;

        #endregion

        #region Identity

        public IReference Reference
        {
            get
            {
                return reference;
            }
            set
            {
                if ((value as FileReference) == reference) return;
                reference = (FileReference)value;
                this.Path = reference.Path;
            }
        } private FileReference reference;

        #region Path

        public string Path
        {
            get { return path; }
            set
            {
                if (path == value) return;
                if (path != default(string)) throw new AlreadyException();

                if (fsw != null)
                {
                    FSW = null;
                }

                path = value;

                if (path == null)
                {
                    fsw = null;
                }
                else
                {
                    fsw = new FileSystemWatcher(System.IO.Path.GetDirectoryName(path), System.IO.Path.GetFileName(path) + ".*");
                    fsw.NotifyFilter = NotifyFilters.LastWrite;
                    
                }
            }
        } private string path;

        #endregion

        #endregion

        #region History

        #region PreviousPaths

        public IEnumerable<string> PreviousPaths
        {
            get { return previousPaths ?? Enumerable.Empty<string>(); }
        }
        private List<string> previousPaths;

        private void OnRenamed(string newPath)
        {
            if(previousPaths == null)
            {
                previousPaths = new List<string>();
            }
            previousPaths.Add(Path);
        }

        #endregion

        #endregion
               
        #region Construction and Destruction

        public FsObjectWatcher()
        {
        }

        public void Dispose()
        {
            FSW = null; // Disposes
        }

        #endregion
        
        #region Events

        /// <summary>
        /// TODO: Eliminate 2nd parameter?
        /// </summary>
        public event Action<IObjectWatcher, IReference> ReferenceChangedFor
        {
            add
            {
                if (referenceChangedFor == null)
                {
                    if (fsw == null) throw new InvalidOperationException("Must first set Reference");
                    fsw.Changed += fsw_Changed;
                    UpdateEnableRaisingEvents(true);
                }
                referenceChangedFor += value;
            }
            remove
            {
                referenceChangedFor -= value;
                if (referenceChangedFor == null)
                {
                    fsw.Changed -= fsw_Changed;
                    UpdateEnableRaisingEvents();
                }
            }
        } event Action<IObjectWatcher, IReference> referenceChangedFor;

        void fsw_Changed(object sender, FileSystemEventArgs e)
        {
            var fileRef = new FileReference(e.FullPath);

            l.Trace("Got change: " + e.ChangeType + " for " + Reference);
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.All:
                    break;
                case WatcherChangeTypes.Changed:
                    ObjectChanged?.Invoke(this.Reference, WatcherChangeType.Modified, null);
                    break;
                case WatcherChangeTypes.Created:
                    break;
                case WatcherChangeTypes.Deleted:
                    break;
                case WatcherChangeTypes.Renamed:

                    var oldRef = this.Reference;

                    OnRenamed(e.FullPath);

                    referenceChangedFor?.Invoke(this, null);                    
                    ObjectChanged?.Invoke(this.Reference, WatcherChangeType.Renamed, oldRef);

                    break;
                default:
                    break;
            }
        }

        public event ObjectChangedHandler ObjectChanged;

        private void UpdateEnableRaisingEvents(bool x = false)
        {
            if (referenceChangedFor != null)
            {
                x = true;
            }
            // FUTURE Add more events here

            fsw.EnableRaisingEvents = x;
        }

        #endregion

        #region Misc

        private static ILogger l = Log.Get();

        #endregion
    }

}
