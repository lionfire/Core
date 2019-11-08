namespace LionFire.Persistence
{
    public struct PersistenceSnapshot : IPersistenceSnapshot<object>
    {
        public PersistenceSnapshot(PersistenceFlags flags, object value, bool? hasValue)
        {
            Flags = flags;
            Value = value;
            HasValue = hasValue ?? value != default;
        }

        public PersistenceFlags Flags { get; set; }
        public bool HasValue { get; set; }
        public object Value { get; set; }
    }

    public struct PersistenceSnapshot<TValue> : IPersistenceSnapshot<TValue>
        where TValue : class // REVIEW
    {
        public PersistenceSnapshot(PersistenceFlags flags, TValue value, bool? hasValue)
        {
            Flags = flags;
            Value = value;
            HasValue = hasValue ?? value != default;
        }

        public PersistenceFlags Flags { get; set; }
        public bool HasValue { get; set; }
        public TValue Value { get; set; }
    }

    //public static class DefaultExtensions
    //{
    //    public static bool IsDefault<T>(this T value)
    //    {
    //        if(typeof(T).IsValueType)
    //        {
    //            return value.IsStructDefault();
    //        }

    //    }
    //    public static bool IsStructDefault<T>(this T value)
    //        where T : struct
    //    {
    //        return System.Collections.Generic.EqualityComparer<T>.Default.Equals(value, default);
    //    }
    //    public static bool IsClassDefault<T>(this T value)
    //        where T : class
    //    {
    //        return value == default;
    //    }
    //}
}
