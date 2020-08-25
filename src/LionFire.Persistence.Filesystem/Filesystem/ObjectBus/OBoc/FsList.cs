#if TOPORT
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LionFire.Collections;

namespace LionFire.ObjectBus.Filesystem
{
    public class FsList : MultiBindableCollection<FsListEntry>
    {
        public string Dir { get; set; }

        public FsList(string dir) { this.Dir = dir; }

        /// <summary>
        /// Last result of Directory.GetFiles()
        /// </summary>
        public IEnumerable<string> Paths { get; private set; }

        private Dictionary<string, FsListEntry> dict = new Dictionary<string, FsListEntry>();

        private object _lock = new object();
        public async Task Refresh()
        {
            //lock (_lock)
            {

                var goneMissing = new Dictionary<string, FsListEntry>(dict);
                var added = new Dictionary<string, FsListEntry>();

                var list = await Task.Run(() => Directory.GetFiles(Dir));

                this.Paths = list;

                foreach (var p in Paths)
                {
                    var name = Path.GetFileName(p);

                    if (goneMissing.Remove(name))
                    {
                        // Was existing already.  Do nothing.
                    }
                    else
                    {
                        // New entry
                        var entry = new FsListEntry
                        {
                            Name = name,
                            Path = p,
                        };
                        dict.Add(name, entry);
                        added.Add(name, entry);
                    }
                }

                foreach(var removal in goneMissing)
                {
                    dict.Remove(removal.Key);
                }

                // TODO: combine these into one operation?
                this.RemoveAll(goneMissing.Values);
                this.AddRange(added.Values);
            }
        }

        public void OnDirectoryDoesNotExist()
        {
        }
    }
}
#endif