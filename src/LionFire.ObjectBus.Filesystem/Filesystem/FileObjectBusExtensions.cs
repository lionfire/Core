namespace LionFire.ObjectBus.Filesystem
{
    public static class FileObjectBusExtensions
    {
        public static object GetObjectFromFile(this string diskPath)
        {
            return OBus.Get(new LocalFileReference(diskPath));
        }
        public static object TryGetObjectFromFile(this string diskPath, OptionalRef<RetrieveInfo> optionalRef = null)
        {
            return OBus.TryGet(new LocalFileReference(diskPath), optionalRef: optionalRef);
        }

        public static void SetObjectAtFile(string diskPath, object value)
        {
            OBus.Set(new LocalFileReference(diskPath), value);
        }
    }
}
