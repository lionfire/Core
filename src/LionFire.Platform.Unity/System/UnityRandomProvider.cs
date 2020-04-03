using UnityRandom = UnityEngine.Random;
using FloatRandom = UnityEngine.Random;
using SystemRandom = System.Random;
using System.Threading;


namespace LionFire
{
    public class UnityRandomProvider : IRandomProvider
    {
        private static int seed = System.Environment.TickCount;

        private static ThreadLocal<SystemRandom> randomWrapper = new ThreadLocal<SystemRandom>(() =>
            new SystemRandom(Interlocked.Increment(ref seed))
        );

        private static ThreadLocal<UnityRandom> randomFloatWrapper = new ThreadLocal<UnityRandom>(() => new UnityRandom());
        //public static FloatRandom ThreadRandomFloat => randomFloatWrapper.Value;
        public float NextFloat() => UnityEngine.Random.value; 

        public static SystemRandom ThreadRandom => randomWrapper.Value;

        public int Next(int min, int max) => ThreadRandom.Next(min, max);
        public int Next(int max) => ThreadRandom.Next(max);
        public int Next() => ThreadRandom.Next();

    }
}
