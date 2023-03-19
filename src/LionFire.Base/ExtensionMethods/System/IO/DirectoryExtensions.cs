using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.ExtensionMethods
{
    public static class DirectoryExtensions
    {
        public static string EnsureDirectoryExists(this string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Console.WriteLine("Creating directory: " + dirPath);
                Directory.CreateDirectory(dirPath);
            }
            return dirPath;
        }
        
    }
}
