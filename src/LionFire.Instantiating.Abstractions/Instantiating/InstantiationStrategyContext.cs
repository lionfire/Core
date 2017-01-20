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
        public SortedList<int, IInstantiationProvider> Strategies { get; set; }

        public static InstantiationStrategyContext Default
        {
            get
            {
                var result = ManualSingleton<InstantiationStrategyContext>.Instance;
                if (result != null) return result;

                result = new InstantiationStrategyContext();

                var strategies = new SortedList<int, IInstantiationProvider>();
                strategies.Add(10, new IToInstantiatorStrategy());
                strategies.Add(20, new IsIInstantiatorStrategy());
                strategies.Add(30, new IProvidesInstantiatorStrategy());
                strategies.Add(40, new IProvidesInstantiatorsStrategy());
                strategies.Add(50, new InstantiatorsFromAttributesStrategy());
                //strategies.Add(60000, new SerializeObjectStrategy());  Use ToInstantiatorOrObject instead 
                result.Strategies = strategies;

                ManualSingleton<InstantiationStrategyContext>.Instance = result;
                return result;
            }
        }
    }
}
