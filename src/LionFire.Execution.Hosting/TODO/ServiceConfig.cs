//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//#if NET461
//#else
//using System.Runtime.Loader;
//#endif
//using System.Threading.Tasks;

//namespace LionFire.Execution.Hosting
//{

//    // TODO:
//    //  - restart/fallback options, 
//    //  - error feedback? notify on failure

//    public class ServiceConfig
//    {
//        public string Key { get { return ServiceType?.FullName ?? ServiceTypeName; } } // TODO: Make unique, maybe a GUID
//        public Guid Guid { get; set; }

//        public string Arg {
//            get { return arg; }
//            set {
//                this.arg = value;
//            }
//        }
//        private string arg;

//#region Construction

//        public ServiceConfig() { }
//        public ServiceConfig(string arg) { this.Arg = arg; }

//        //public ServiceConfig(string package, string type, string parameter = null) { }

//#endregion



//#region Hosting Configuration
//        public ServiceConfigFlags Flags { get; set; }

//#endregion

//#region Service Parameters

//        public Dictionary<string, object> Parameters { get; set; }

//#endregion

//#region Runtime Location


//        public string Hive { get; set; }

//        /// <summary>
//        /// If null, the configuraton for the ServiceHost configuration will be used.
//        /// </summary>
//        public string Host { get; set; }

//        public string ProcessName { get; set; }

//        public ExecutionLocationType ExecutionLocationType { get; internal set; } = ExecutionLocationType.InProcess;

//#endregion

//#region Runtime Parameters

//        public string Platform { get; set; } = "NetCoreApp1.0"; // MOVE Default hardcoded string

//        #endregion

//        #region Identifier Parameters


//        public bool IsExecutionResolved { // REVIEW - split into separate classes?
//            get {
//                return Script != null || ServiceType != null;
//            }
//        }

        


//        #endregion

 
//    }


//}
