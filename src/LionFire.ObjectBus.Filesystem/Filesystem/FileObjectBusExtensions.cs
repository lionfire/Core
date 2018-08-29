namespace LionFire.ObjectBus.Filesystem
{
    public static class FileObjectBusExtensions
    {
        public static object GetObjectFromFile(this string diskPath)
        {
            return OBus.Get(new FileReference(diskPath));
        }
        public static object TryGetObjectFromFile(this string diskPath, OptionalRef<RetrieveInfo> optionalRef = null)
        {
            return OBus.TryGet(new FileReference(diskPath), optionalRef: optionalRef);
        }

        public static void SetObjectAtFile(string diskPath, object value)
        {
            OBus.Set(new FileReference(diskPath), value);
        }
    }
#endif
}
