using System;
#if UNITY
using UnityRandom = UnityEngine.Random;
using FloatRandom = UnityEngine.Random;
#else
using FloatRandom = System.Random;
#endif
using SystemRandom = System.Random;
using System.Threading;


namespace LionFire
{

    // Retrieved from http://csharpindepth.com/Articles/Chapter12/Random.aspx
    // as public domain.
    
    // SECURITY: Author warns that this is not secure.
    public static class RandomProvider
    {
        private static int seed = Environment.TickCount;

        private static ThreadLocal<SystemRandom> randomWrapper = new ThreadLocal<SystemRandom>(() =>
            new SystemRandom(Interlocked.Increment(ref seed))
        );

#if UNITY
        private static ThreadLocal<UnityRandom> randomFloatWrapper = new ThreadLocal<UnityRandom>(() => new UnityRandom());
#endif

        public static SystemRandom ThreadRandom
        {
            get
            {
                return randomWrapper.Value;
            }
        }

        public static FloatRandom ThreadRandomFloat
        {
            get
            {
#if UNITY
                return randomFloatWrapper.Value;
#else
                return randomWrapper.Value;
#endif
            }
        }
    }
}
