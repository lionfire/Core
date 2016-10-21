using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Execution.Composition;

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
        public static T AddConfigurer<T>(this T composableExecutable, Action<T> configure)
            where T : class, IComposableExecutable<T>
        {
            composableExecutable.Add(new Configurer<T>(configure));
            return composableExecutable;
        }
    }
}
