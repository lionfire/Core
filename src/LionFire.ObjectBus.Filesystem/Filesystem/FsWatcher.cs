using LionFire.ObjectBus.Filesystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus.Filesystem
{
    public class FsWatcher : IObjectWatcher
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
                if (path != default(string)) throw new AlreadySetException();

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

        #region Construction

        public FsWatcher()
        {
        }

        public void Dispose()
        {
            FSW = null; // Disposes
        }

        #endregion


        private void UpdateEnableRaisingEvents(bool x = false)
        {
            if (referenceChangedFor != null)
            {
                x = true; 
            }
            // FUTURE Add more events here

            fsw.EnableRaisingEvents = x; 
        }
        #region Events

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
            var ev = referenceChangedFor;
            if (ev != null) ev(this, fileRef);
        }

        #endregion

        #region Misc

        private static ILogger l = Log.Get();

        #endregion
    }

}
