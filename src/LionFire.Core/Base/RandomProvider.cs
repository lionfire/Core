using LionFire.Dependencies;

namespace LionFire.Structures
{
    public static class RandomProvider
    {
        public static IRandomProvider Instance => ServiceLocator.Get<IRandomProvider>();
    }
}
