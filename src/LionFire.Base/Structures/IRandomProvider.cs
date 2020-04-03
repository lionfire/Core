#if UNITY
#else
using FloatRandom = System.Random;
#endif


namespace LionFire
{
    public interface IRandomProvider
    {

        int Next(int min, int max);
        int Next(int max);
        int Next();
        float NextFloat();
    }
}
