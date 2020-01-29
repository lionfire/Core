using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace LionFire.Services
{
    public static class RunFileManager
    {
        #region RunFileDirectory

        public static string RunFileDirectory
        {
            get
            {
                if (runFileDirectory == null)
                {
                    runFileDirectory = LionFireEnvironment.Directories.Other["LionRun"];
                }
                return runFileDirectory;
            }
            set => runFileDirectory = value;
        }
        static string runFileDirectory = null;

        #endregion


        public static void CleanupInvalidRunFiles()
        {
            List<RunFile> runFiles = new List<RunFile>();

            foreach (string filePath in Directory.GetFiles(RunFileDirectory, "*." + RunFile.Extension))
            {
                runFiles.Add(RunFile.FromPath(filePath));
            }

            foreach (RunFile runFile in runFiles)
            {
                runFile.DeleteIfInvalid();
            }

        }

        public static List<RunFile> GetInstances(string runClassName = null)
        {
            if (runClassName != null && runClassName.Contains(RunFile.Separator)) throw new ArgumentException("Class name cannot contain " + RunFile.Separator);

            CleanupInvalidRunFiles();

            List<RunFile> runFiles = new List<RunFile>();

            if(!Directory.Exists(RunFileDirectory)) return runFiles;

            foreach (string filePath in Directory.GetFiles(RunFileDirectory, (runClassName??"*") + RunFile.Separator + "*." + RunFile.Extension))
            {
                runFiles.Add(RunFile.FromPath(filePath));
            }

            return runFiles;
        }

    }
}
