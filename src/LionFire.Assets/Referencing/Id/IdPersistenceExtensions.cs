using LionFire.Data.Id;
using LionFire.Persistence;
using LionFire.Resolves;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.ExtensionMethods.Persistence;

namespace LionFire.ExtensionMethods.Persistence
{
    public static class IdedSaveExtensions
    {
        public static Task<IPersistenceResult> Save<T>(this T ided)
          where T : IIded<string> 
            => ided.GetReadWriteHandle<T>().Save();

        public static Task<IPersistenceResult> TrySave<T>(this T ided)
            where T : IIded<string>
            => ided.GetReadWriteHandle<T>().TrySave();
    }
}
