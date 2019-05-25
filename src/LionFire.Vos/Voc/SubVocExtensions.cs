#if TOPORT
namespace LionFire.Vos
{
    public static class SubVocExtensions
    {
        public static Vob GetSubCollection<T>(this Vob vob, string subpath = null)
            where T : class, new()
        {
            subpath = subpath ?? Voc<T>.DefaultSubpath;
            return vob[subpath];
        }

#if VOC
        public static Voc<T> GetSubCollectionVoc<T>(this Vob vob, string subpath = null)
            where T : class, new()
        {
            return vob.GetSubCollection<T>(subpath).GetVoc<T>();
        }
#endif
    }
}
#endif