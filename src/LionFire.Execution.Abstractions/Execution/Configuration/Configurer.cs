
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Execution.Composition;
using LionFire.Structures;
using LionFire.Composables;

namespace LionFire.Execution.Composition
{

    public class Configurer<T> : IConfigures<T>
     where T : class
    {

        public Configurer(Action<T> configMethod)
        {
            this.ConfigureMethod = configMethod;
        }

        public Action<T> ConfigureMethod { get; set; }

        public void Configure(T target)
        {
            ConfigureMethod(target);
        }
    }
}

namespace LionFire.Execution
{
    public static class ConfigurerExtensions
    {
        public static T AddConfigurer<T>(this T composable, Action<T> configure)
            where T : class, IComposable<T>
        {
            composable.Add(new Configurer<T>(configure));
            return composable;
        }
    }
}
