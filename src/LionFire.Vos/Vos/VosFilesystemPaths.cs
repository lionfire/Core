//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;

//namespace LionFire.ObjectBus
//{
//    /// <summary>
//    /// Provides some default mount paths in typical OS-specific locations
//    /// </summary>
//    public static class VosFilesystemPaths
//    {
        

//        /// <summary>
//        /// Per-user data
//        /// </summary>
//        public static string ApplicationData
//        {
//            get
//            {
//                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), LionEnvironment.LionAppName);
//            }
//        }
        
//        public static string LocalApplicationData
//        {
//            get
//            {
//                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), LionEnvironment.LionAppName);
//            }
//        }

//        public static string CommonApplicationData
//        {
//            get
//            {
//                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), LionEnvironment.LionAppName);
//            }
//        }
//    }
//}
