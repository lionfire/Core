using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.CommandLine.Dispatching
{
    public enum UsageOptions
    {
        Before,
        After,

        /// <summary>
        /// Only show if application lacks a "usage"
        /// </summary>
        OnlyIfMissing,
    }

    public class CliDispatcherOptions
    {
        /// <summary>
        /// Add these handlers if present
        /// </summary>
        public List<string> AdditionalHandlerTypeNames { get; set; } = new List<string>
        {
            "LionFire.Environment.CommandLine.LionEnvironmentCliHandlers, LionFire.Environment.CommandLine, Version=0.1.0.0"
        };

        public List<Type> AdditionalHandlerTypes { get; set; } = new List<Type>();


        /// <summary>
        /// If no verbs match, the default verb will be this one, if it is implemented.
        /// </summary>
        public string DefaultUsageLongForm { get; set; } = "usage";

        public UsageOptions ShowDefaultUsage = UsageOptions.After;

    }

}
