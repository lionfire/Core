namespace LionFire.Persistence.Handles
{
    public static class ObjectHandleExtensions
    {

        public static ObjectHandle<TValue> ToObjectHandle<TValue>(this TValue value) => new ObjectHandle<TValue>(value);
    }
}
