using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus.Filesystem
{
  public class FsOBaseSettings
    {
        //// OLD - delete once in VosPath
        //public const char FileTypeDelimiter = '(';
        //public const char FileTypeEndDelimiter = ')';

        public const bool AppendTypeNameToFileNames = false; // TEMP - TODO: Figure out a way to do this in VOS land

        internal static string GetTypeNameFromFileName(string fileName)
        {
            throw new NotImplementedException();
#if TOPORT
            int index = fileName.IndexOf(VosPath.TypeDelimiter);
            if (index == -1) return null;
            return fileName.Substring(index, fileName.IndexOf(VosPath.TypeEndDelimiter, index) - index);
#endif
        }
    }
}
