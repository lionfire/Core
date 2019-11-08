namespace LionFire.Persistence.Handles
{
    // REVIEW TODESIGN

    public static class IHandleBaseExtensions
    {
        public static T Object<T>(this IHandleBase handleBase)
        {
            return (T)IHandleBaseExtensionsCache<T>.get_ObjectPropertyInfo.GetValue(handleBase);
        }
    }
}
