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

    public static class DefaultInstantiationStrategy
    {
        static InstantiationStrategyContext defaultInstantiationStrategyContext;

        // TODO: Run this by default in apps using DLL Initializers
        public static InstantiationStrategyContext Initialize()
        {
            //var result = ManualSingleton<InstantiationStrategyContext>.Instance;
            if (defaultInstantiationStrategyContext != null) return defaultInstantiationStrategyContext;

            defaultInstantiationStrategyContext = new InstantiationStrategyContext();

            var strategies = new SortedList<int, IInstantiationProvider>();
            strategies.Add(10, new IToInstantiatorStrategy());
            strategies.Add(20, new IsIInstantiatorStrategy());
            strategies.Add(30, new IProvidesInstantiatorStrategy());
            strategies.Add(40, new IProvidesInstantiatorsStrategy());
            strategies.Add(50, new InstantiatorsFromTemplateAndStateStrategy());
            //strategies.Add(60000, new SerializeObjectStrategy());  Use ToInstantiatorOrObject instead 
            defaultInstantiationStrategyContext.Strategies = strategies;

            //ManualSingleton<InstantiationStrategyContext>.Instance = defaultInstantiationStrategyContext;

            DependencyLocator.Set(defaultInstantiationStrategyContext);
            return defaultInstantiationStrategyContext;
        }
    }

    
}
