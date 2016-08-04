using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.PlatformAbstractions;
using LionFire.ExtensionMethods;


namespace LionFire
{

    public partial class LionEnvironment
    {
        #region Compile Environment

        public static string CompileTargetFrameworkMoniker {
            get {
#if NET451
                           return "NET451";
#elif NET462
                    return "NET462";
#elif NETCOREAPP1_0
                return "NETCOREAPP1_0";
#else
                throw new NotImplementedException();
#endif
            }
        }

        #endregion

        #region Runtime Environment

        //PlatformServices.Default.Application.RuntimeFramework

        #endregion

        #region Program Environment

        public static string ProgramName { get { return "LionFireProgram(TODO)"; } }

        public static string ProgramVersion { get { return "0.0.0-alpha"; } }

        #endregion

        public static TextWriter StandardOutput { get; private set; }

        public static Stream StandardOutputStream {
            get { return standardOutputStream; }
            set {
                standardOutputStream = value;
                StandardOutput = new StreamWriter(value);
            }
        }
        private static Stream standardOutputStream;

        #region Construction

        static LionEnvironment()
        {
            Directories.EnsureAllDirectoriesExist();
            StandardOutputStream = Console.OpenStandardOutput();
        }

        #endregion

    }
}
