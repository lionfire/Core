using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace System.IO
{
    public static class LionFireDirectoryExtensions
    {
        public static string EnsureDirectoryExists(this string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            return dirPath;
        }
        
    }
}
