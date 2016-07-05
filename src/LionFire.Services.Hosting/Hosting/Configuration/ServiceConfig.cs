using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Services.Hosting
{

    // TODO:
    //  - restart/fallback options, 
    //  - error feedback? notify on failure

    public class ServiceConfig
    {
        #region Construction

        public ServiceConfig() { }
        public ServiceConfig(string package, string type, string parameter = null) { }

        #endregion

        #region Service Reference

        public string Package { get; set; }

        public string ClassName { get; set; }

        #endregion

        #region Hosting Configuration
        public ServiceConfigFlags Flags { get; set; }

        #endregion

        #region Service Parameters
        
        public Dictionary<string, object> Parameters { get; set; }
        
        #endregion
        
        #region Runtime Location


        public string Hive { get; set; }

        /// <summary>
        /// If null, the configuraton for the ServiceHost configuration will be used.
        /// </summary>
        public string Host  { get; set; }

        public string ProcessName { get; set; }

        #endregion

        #region Runtime Parameters

        public string Platform { get; set; } = "NetCoreApp1.0"; // MOVE Default hardcoded string

        #endregion
        

    }
}
