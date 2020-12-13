using LionFire.UI;

namespace LionFire.UI
{
    public static class IUICollectionExtensions
    {
        public static T GetOrCreate<T>(this IUICollection collection, string key)
            where T : IUIObject
        {
            return default;
        }

        public static bool Remove(this IUICollection collection, IUIKeyed keyed) => collection.Remove(keyed.Key);

        public static void Close(this IUICollection collection)
        {
            collection.RemoveAll();
            collection.RemoveFromParent();
        }
    }
}
