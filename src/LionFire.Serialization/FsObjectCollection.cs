using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using LionFire.Execution;
using System.IO;
using System.Diagnostics;
using LionFire.Serialization.Contexts;
using LionFire.Handles;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections.Generic;
using LionFire.ExtensionMethods;
using LionFire.Structures;

namespace LionFire.Serialization
{
    /// <summary>
    /// A general-purpose collection of objects, with change events.
    /// FUTURE: Also use this for Voc 2.0?
    /// </summary>
    public interface IPersistedObjectCollection<T>
    //: INotifyCollectionChanged
    {
        string RootPath { get; set; }

        //IReadOnlyDictionary<string, IReadHandle<T>> Handles { get; }
        //IReadOnlyCollection<IReadHandle<T>> Objects { get; }
    }
    public class FsObjectCollection<T> : ObservableHandleDictionary<string, SerializingFileHandle<T>, T>
        //ObservableDictionary<string, IReadHandle<T>>, 
        , IPersistedObjectCollection<T>
        , INotifyPropertyChanged
        where T : class
    {

        #region Construction

        public FsObjectCollection() { }
        public FsObjectCollection(string path)
        {
            this.RootPath = path;
        }

        //private void OnHandlesChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    this.Changed

        //    if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
        //    {
        //    }
        //    else
        //    {
        //        if (e.NewItems != null)
        //        {
        //            foreach (var item in e.NewItems)
        //            {

        //            }
        //        }
        //        if (e.OldItems != null)
        //        {
        //            foreach (var item in e.OldItems)
        //            {
        //            }
        //        }
        //    }
        //    throw new NotImplementedException("TODO: propagate INCC");
        //    //foreach (var n in e.NewItems)
        //    //{

        //    //}
        //}

        #endregion

        #region State

        private FileSystemWatcher fsw;

        #endregion

        #region IObjectCollection

        #region RootPath

        public string RootPath
        {
            get { return rootPath; }
            set
            {
                if (rootPath == value) return;
                if (fsw != null && (value == null || !Directory.Exists(value)))
                {
                    fsw.EnableRaisingEvents = false;
                    fsw.Changed -= OnChanged;
                    fsw.Created -= OnCreated;
                    fsw.Deleted -= OnDeleted;
                    fsw.Error -= OnError;
                    fsw.Dispose();
                    fsw = null;
                }

                rootPath = value;

                if (fsw == null && (value != null && Directory.Exists(value)))
                {
                    fsw = new FileSystemWatcher(rootPath, SearchPattern ?? "*");

                    fsw.Changed += OnChanged;
                    fsw.Created += OnCreated;
                    fsw.Deleted += OnDeleted;
                    fsw.Error += OnError;
                    fsw.EnableRaisingEvents = true;
                }
                else
                {
                    fsw.Path = rootPath;
                }

                RefreshHandles();

                OnPropertyChanged(nameof(RootPath));
            }
        }
        private string rootPath;

        #endregion

        #endregion

        public bool IsStarted => fsw != null;


        #region SearchPattern

        public string SearchPattern
        {
            get { return searchPattern; }
            set
            {
                searchPattern = value;
                if (fsw != null)
                {
                    fsw.Filter = searchPattern;
                }
            }
        }
        private string searchPattern;

        #endregion

        public ISerializer Serializer { get; set; }


        // FUTURE: Objects -- attaching to it auto-retrieves the handles.

        // FUTURE:
        //public bool AutoLoad { get; set; }
        public override void RefreshHandles()
        {
            var newList = new Dictionary<string, SerializingFileHandle<T>>();

            if (RootPath != null && Directory.Exists(RootPath))
            {
                foreach (var file in Directory.GetFiles(RootPath, SearchPattern ?? "*"))
                {
                    var h = new SerializingFileHandle<T>(file);
                    newList.Add(h);
                }
            }

            handles.SetToMatch(newList);
        }

        public override string KeyToHandleKey(string key)
        {
            return Path.Combine(this.RootPath, key);
        }
        public override string HandleKeyToKey(string key)
        {
            if (key.StartsWith(RootPath))
            {
                return key.Substring(RootPath.Length);
            }
            else
            {
                throw new ArgumentException("Key does not start with RootPath");
            }
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            Debug.WriteLine(e.GetException()?.ToString());
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            Debug.WriteLine("Deleted: " + e.FullPath);
            handles.Remove(e.FullPath);
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            Debug.WriteLine("Created: " + e.FullPath);
            OnCreatedOrChanged(e.FullPath);
        }

        private void OnCreatedOrChanged(string path)
        {
            if (!handles.ContainsKey(path))
            {
                var h = new SerializingFileHandle<T>(path);
                handles.Add(path, h);
            }
            else
            {
                handles[path].ForgetObject();
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            Debug.WriteLine("Changed: " + e.FullPath);
            OnCreatedOrChanged(e.FullPath);
        }

        #region Misc


        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #endregion
    }

}
