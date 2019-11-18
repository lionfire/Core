using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.MultiTyping;
using LionFire.Structures;
using LionFire.States;
using System.Reflection;
using LionFire.Instantiating;
using LionFire.ExtensionMethods;
using LionFire.Dependencies;

namespace LionFire.Instantiating
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class InstantiatorTypeAttribute : Attribute
    {
        public Type Type { get { return type; } }
        Type type;
        public InstantiatorTypeAttribute(Type type) { this.type = type; }
    }
    
    public class InstantiationStrategyContext
    {
        /// <summary>
        /// Reserved: Below 100 and above 60000
        /// </summary>
        public SortedList<int, IInstantiationProvider> Strategies { get; set; } = new SortedList<int, IInstantiationProvider>();

        #region Static

        public static InstantiationStrategyContext Default => DependencyLocator.Get<InstantiationStrategyContext>();

        #endregion
    }
}
