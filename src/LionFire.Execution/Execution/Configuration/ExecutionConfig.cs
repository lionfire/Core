using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Execution.Configuration
{

    /// <summary>
    /// The Uri that summarizes the request for execution.  
    /// Examples:
    ///  - gist:lionfire/SampleScript.csscript.cs
    ///  - gist:lionfire/SampleScript.roslyn.cs !roslyn
    ///  - github:username/repo/path/to/project(Class
    ///  - nuget:LionFire.Services.Samples(SampleService)
    ///  - nuget:LionFire.Services.Samples  // Looks up default class in Assembly attributes, or else class Service in root namespace, else class Program.
    ///  - file://e:/src/test.cs!roslyn@@myhive?priority=5&autoupdate=true
    /// </summary>
    public class ExecutionConfig
    {
        #region Properties

        public string Runtime { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public object Arguments { get; internal set; } // REVIEW
        public string SourceUriScheme { get; internal set; }
        public string ResolvedSourceUri { get; internal set; }
        public object SourceContent { get;  set; }

        public string SourceUriBody { get; set; }
        public string ExecutionLocation { get; set; }
        public ExecutionLocationType ExecutionLocationType { get; set; }

        #region In-process object hosting

        public string TypeName { get; set; }

        public Type Type { get; set; }

        public object Object { get; set; }

        #endregion

        #region Execution Type

        public ExecutionKind ExecutionType {
            get; set;
        }

        #endregion

        public string ConfigName { get; internal set; }

        #endregion

        #region Construction

        public ExecutionConfig() { }
        public ExecutionConfig(string specifier) { this.ParseSpecifier(specifier); }

        public static implicit operator ExecutionConfig(string specifier)
        {
            var config = new ExecutionConfig();
            config.ParseSpecifier(specifier);
            return config;
        }

        #endregion

        #region From ServiceConfig REVIEW

        #region Service Reference

        public string PackageSchema { get; set; }

        public string Package { get; set; }



        #endregion

        #region Assembly

        public Assembly Assembly { get; set; }

        #region AssemblyName

        public string AssemblyName {
            get { return assemblyName; }
            set { assemblyName = value; }
        }
        private string assemblyName;

        #endregion

        #region AssemblyVersion

        public string AssemblyVersion {
            get { return assemblyVersion; }
            set { assemblyVersion = value; }
        }
        private string assemblyVersion;

        #endregion


        /// <summary>
        /// Single file script file, for Roslyn
        /// </summary>
        public string Script { get { return SourceContent as string; } } // REVIEW - eliminate this, move to roslyn dll

        #endregion


        #endregion

    }


}
