using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;

#if RAPTORDB
using LionFire.ObjectBus.RaptorKV;
#endif

namespace LionFire.ObjectBus
{
    public static class OBusConfig
    {
        private static bool isInitialized = false;
        private static object initializingLock = new object();

        public static void Initialize()
        {
            lock (initializingLock)
            {
                if (isInitialized) return;
                isInitialized = true;

#if RAPTORDB
                SchemeBroker.Instance.Register(RkvOBaseProvider.Instance);
#endif


                //VosMountManager.Instance.Mount("/fs", "file:///");

                //VosMountManager.Instance.Mount("/UserData", "file:///"); // TODO: OS-specific User dir
                //VosMountManager.Instance.Mount("/AppData", "file:///"); // TODO: App dir

                //VosMountManager.Instance.Mount(new MountOptions
                //{
                //    MountPath = "/fs",
                //    ConnectionString = "",
                //});

            }
        }

        private static ILogger l = Log.Get();


    }
}
