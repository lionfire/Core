#if UNITY
using FloatRandom = UnityEngine.Random;
#else
using FloatRandom = System.Random;
#endif
using SystemRandom = System.Random;
using System.Threading;


namespace LionFire;

public class NetStandardRandomProvider : IRandomProvider
{
    private static int seed = System.Environment.TickCount;

    private static ThreadLocal<SystemRandom> randomWrapper = new ThreadLocal<SystemRandom>(() =>
        new SystemRandom(Interlocked.Increment(ref seed))
    );

    public static SystemRandom ThreadRandom => randomWrapper.Value;

    public static FloatRandom ThreadRandomFloat => randomWrapper.Value;

    public int Next(int min, int max) => ThreadRandom.Next(min, max);
    public int Next(int max) => ThreadRandom.Next(max);
    public int Next() => ThreadRandom.Next();
    public float NextFloat() => (float) ThreadRandomFloat.NextDouble(); // CAST
}
