using LionFire.Execution.Composition;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Composables;

namespace LionFire.Execution.Composition
{
    public class Initializer<T> : IInitializes<T>
        where T : class
    {

        public Initializer(Func<T, Task<bool>> initializeMethod)
        {
            this.InitializeMethod = initializeMethod;
        }

        public Func<T, Task<bool>> InitializeMethod { get; set; }

        public async Task<bool> Initialize(T target)
        {
            return await InitializeMethod(target).ConfigureAwait(false);
        }
    }
}

namespace LionFire.Execution
{
    public static class InitializerExtensions
    {
        public static T InitializeWith<T>(this T composable, Func<T, Task<bool>> tryInitialize)
            where T : class, IComposable<T>
        {
            composable.Add(new Initializer<T>(tryInitialize));
            return composable;
        }
    }
}
