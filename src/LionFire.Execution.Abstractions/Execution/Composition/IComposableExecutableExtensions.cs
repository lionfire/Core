using LionFire.Execution.Composition;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    public static class IComposableExecutableExtensions
    {
        public static T AddConfig<T,TConfig>(this T composable, Action<TConfig> tryInitialize)
            where T : class, IComposableExecutable<T>
            where TConfig : class
        {
            composable.Add(new Configurer<TConfig>(tryInitialize));
            return composable;
        }

        public static T ConfigureServices<T>(this T composable, Action<IServiceCollection> configure)
            where T : class, IComposableExecutable<T>
        {
            composable.Add(new Configurer<IServiceCollection>(sc => configure(sc)));
            return composable;
        }
    }
}
