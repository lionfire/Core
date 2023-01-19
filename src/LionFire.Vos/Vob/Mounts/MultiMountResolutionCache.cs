using LionFire.Collections;
using LionFire.Referencing;
using System;
using System.Collections.Generic;

namespace LionFire.Vos.Mounts
{
    public struct MultiMountResolutionCache : IMountResolutionCache
    {
        public Vob Vob { get; }
        public MultiMountResolutionCache(Vob vob, int version, IEnumerable<IMount> mounts, PersistenceDirection persistenceDirection)
        {
            Vob = vob;
            Version = version;
            sortedMounts = Import(mounts, persistenceDirection, autoResolveDuplicatePriorities: VosStaticOptions.AutoResolveDuplicateMountPriorities);
        }

        public int Version { get; }

        public IEnumerable<KeyValuePair<int, IMount>> Mounts => sortedMounts
            .SelectMany(kvp => kvp.Value.Select(v => new KeyValuePair<int, IMount>(kvp.Key, v)));

        MultiValueSortedList<int, IMount> sortedMounts;

        private static MultiValueSortedList<int, IMount> Import(IEnumerable<IMount> mountsToImport, PersistenceDirection persistenceDirection, bool autoResolveDuplicatePriorities = false)
        {
            var list = new MultiValueSortedList<int, IMount>();
            foreach (var mountToImport in mountsToImport)
            {
                Import(list, mountToImport, persistenceDirection, autoResolveDuplicatePriorities);
            }
            return list;
        }

        public static MultiValueSortedList<int, IMount> Import(MultiValueSortedList<int, IMount> list, IMount mountToImport, PersistenceDirection persistenceDirection, bool autoResolveDuplicatePriorities = false)
        {
            //if (mount.MountOptions.NoSubdirectories && )
            int key;
            switch (persistenceDirection)
            {
                case PersistenceDirection.Read:
                    key = mountToImport.Options.ReadPriority ?? 0;
                    while (autoResolveDuplicatePriorities && list.ContainsKey(key)) key++;
                    list.Add(key, mountToImport);
                    break;
                case PersistenceDirection.Write:
                    key = mountToImport.Options.WritePriority ?? 0;
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
