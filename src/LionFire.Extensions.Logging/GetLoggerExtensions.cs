//// MOVED to LionFire.Runtime.Extensions
//using LionFire.Structures;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.Extensions.DependencyInjection;


//namespace LionFire.Extensions.Logging
//{

//    public static class GetLoggerExtensions
//    {
//        public static Func<string, ILogger> GetLoggerMethod;

//        public static ILogger GetLogger(this object obj, string name = null, bool isEnabled = true)
//        {
//            if (!isEnabled) return Singleton<NullLogger>.Instance;

//            if (name == null) name = obj.GetType().FullName;

//            if (GetLoggerMethod != null)
//            {
//                return GetLoggerMethod(name);
//            }

//            var fac = Defaults.TryGet<ILoggerFactory>();
//            if (fac != null)
//            {
//                return fac.CreateLogger(name);
//            }

//            return Singleton<NullLogger>.Instance;
//        }
//    }
//}
