namespace LionFire.FlexObjects
{
    public interface IHasFlexDictionary<TKey>
    {
        FlexDictionary<TKey> FlexDictionary { get; }
    }

    public static class IHasFlexDictionaryExtensions
    {
        public static IFlex Get<TKey>(IHasFlexDictionary<TKey> dict, TKey key)
            => dict.FlexDictionary.Values[key];


        public static FlexDictionary<TKey> GetFlexDictionaryParent<TKey>(IHasFlexDictionary<TKey> dict, TKey key)
            => ((dict.FlexDictionary as IFlexOverlayOwner ?? dict as IFlexOverlayOwner).ParentFlex as IHasFlexDictionary<TKey>)?.FlexDictionary;

        //public static IFlex this[this IHasFlexDictionary<string> d, string key]
        //{
        //}
    }
}

