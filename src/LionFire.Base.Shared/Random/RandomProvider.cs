#if false  // Use LionFire.Base's RandomProvider instead
using System;
#if UNITY
using UnityRandom = UnityEngine.Random;
using FloatRandom = UnityEngine.Random;
#else
using FloatRandom = System.Random;
#endif
using SystemRandom = System.Random;
using System.Threading;
using LionFire.Dependencies;

namespace LionFire
{

    // Retrieved from http://csharpindepth.com/Articles/Chapter12/Random.aspx
    // as public domain.

    // SECURITY: Author warns that this is not secure.
    public static class RandomProvider
    {
        //        private static int seed = System.Environment.TickCount;

        //        private static ThreadLocal<SystemRandom> randomWrapper = new ThreadLocal<SystemRandom>(() =>
        //            new SystemRandom(Interlocked.Increment(ref seed))
        //        );

        //#if UNITY
        //        private static ThreadLocal<UnityRandom> randomFloatWrapper = new ThreadLocal<UnityRandom>(() => new UnityRandom());
        //#endif

        //        public static SystemRandom ThreadRandom => randomWrapper.Value;

        //        public static FloatRandom ThreadRandomFloat
        //        {
        //            get
        //            {
        //#if UNITY
        //                return randomFloatWrapper.Value;
        //#else
        //                return randomWrapper.Value;
        //#endif
        //            }
        //        }

        public static IRandomProvider Instance => instance ??= ServiceLocator.Get<IRandomProvider>();
        private static IRandomProvider instance;
        public static int Next(int min, int max) => Instance.Next(min, max);

        #region 

        // REVIEW - would I want non-threadsafe providers?

        public static IRandomProvider ThreadRandom => Instance;
        public static IRandomProvider ThreadRandomFloat => Instance;

        #endregion
    }
}
#endif