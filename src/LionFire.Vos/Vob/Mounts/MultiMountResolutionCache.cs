using LionFire.Collections;
using LionFire.Referencing;
using System;
using System.Collections.Generic;

namespace LionFire.Vos
{
    public struct MultiMountResolutionCache : IMountResolutionCache
    {
        public Vob Vob { get; }
        public MultiMountResolutionCache(Vob vob, int version, IEnumerable<Mount> mounts, PersistenceDirection persistenceDirection)
        {
            Vob = vob;
            Version = version;
            sortedMounts = Import(mounts, persistenceDirection, autoResolveDuplicatePriorities: VosStaticOptions.AutoResolveDuplicateMountPriorities);
        }

        public int Version { get; }

        public IEnumerable<KeyValuePair<int, Mount>> Mounts => (IEnumerable<KeyValuePair<int, Mount>>)sortedMounts;
        MultiValueSortedList<int, Mount> sortedMounts;

        private static MultiValueSortedList<int, Mount> Import(IEnumerable<Mount> mountsToImport, PersistenceDirection persistenceDirection, bool autoResolveDuplicatePriorities = false)
        {
            var list = new MultiValueSortedList<int, Mount>();
            foreach (var mountToImport in mountsToImport)
            {
                Import(list, mountToImport, persistenceDirection, autoResolveDuplicatePriorities);
            }
            return list;
        }

        public static MultiValueSortedList<int, Mount> Import(MultiValueSortedList<int, Mount> list, Mount mountToImport, PersistenceDirection persistenceDirection, bool autoResolveDuplicatePriorities = false)
        {
            //if (mount.MountOptions.NoSubdirectories && )
            int key;
            switch (persistenceDirection)
            {
                case PersistenceDirection.Read:
                    key = mountToImport.MountOptions.ReadPriority ?? 0;
                    while (autoResolveDuplicatePriorities && list.ContainsKey(key)) key++;
                    list.Add(key, mountToImport);
                    break;
                case PersistenceDirection.Write:
                    key = mountToImport.MountOptions.WritePriority ?? 0;
                    while (autoResolveDuplicatePriorities && list.ContainsKey(key)) key++;
                    list.Add(key, mountToImport);
                    break;
                default:
                    throw new ArgumentException($"{nameof(persistenceDirection)} must be Read or Write");
            }
            return list;
        }
    }

}
