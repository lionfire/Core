using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.MultiTyping;
using LionFire.Structures;

namespace LionFire.Instantiating
{
    public class InstantiationStrategyContext
    {
        public SortedList<int, IInstantiationProvider> Strategies { get; set; }

        public static InstantiationStrategyContext Default
        {
            get
            {
                var result = ManualSingleton<InstantiationStrategyContext>.Instance;
                if (result != null) return result;

                result = new InstantiationStrategyContext();

                var strategies = new SortedList<int, IInstantiationProvider>();
                strategies.Add(10, new IProvidesInstantiatorStrategy());
                strategies.Add(20, new IProvidesInstantiatorsStrategy());
                result.Strategies = strategies;

                ManualSingleton<InstantiationStrategyContext>.Instance = result;
                return result;
            }
        }
    }
}
