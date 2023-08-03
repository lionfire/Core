namespace LionFire.Data.Async.Gets.ChainResolving
{
    public struct ChainGetterResult<T>
    {
        public bool IsSuccess { get; set; }
        public T ResolvedValue { get; set; }

        /// <summary>
        /// True if a partial resolution can be stored, instead of the initially requested value.  Example: URI strings may be replaced with strongly typed References this way.
        /// </summary>
        public bool HasNewSourceValue { get; set; }
        public object NewSourceValue { get; set; }

        public static implicit operator T (ChainGetterResult<T> r) => r.ResolvedValue;
    }
}
